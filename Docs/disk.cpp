/*
* UAE - The Un*x Amiga Emulator
*
* Floppy disk emulation
*
* Copyright 1995 Hannu Rummukainen
* Copyright 1995-2001 Bernd Schmidt
* Copyright 2000-2003 Toni Wilen
*
* High Density Drive Handling by Dr. Adil Temel (C) 2001 [atemel1@hotmail.com]
*
*/

#include "sysconfig.h"
#include "ersatz.h"
#include "osemu.h"
#include "execlib.h"
#ifdef FDI2RAW
#include "fdi2raw.h"
#endif
#ifdef CAPS
#include "caps/caps_win32.h"
#endif
#include "amax.h"

#undef CATWEASEL

static int longwritemode = 0;

/* support HD floppies */
#define FLOPPY_DRIVE_HD
/* writable track length with normal 2us bitcell/300RPM motor, 12667 PAL, 12797 NTSC */
#define FLOPPY_WRITE_LEN (currprefs.floppy_write_length > 256 ? currprefs.floppy_write_length / 2 : (currprefs.ntscmode ? (12798 / 2) : (12668 / 2)))
#define FLOPPY_WRITE_MAXLEN 0x3800
/* This works out to 350 */
#define FLOPPY_GAP_LEN (FLOPPY_WRITE_LEN - 11 * 544)
/* (cycles/bitcell) << 8, normal = ((2us/280ns)<<8) = ~1830 */
#define NORMAL_FLOPPY_SPEED (currprefs.ntscmode ? 1810 : 1829)
/* max supported floppy drives, for small memory systems */
#define MAX_FLOPPY_DRIVES 4

#ifdef FLOPPY_DRIVE_HD
#define DDHDMULT 2
#else
#define DDHDMULT 1
#endif
#define MAX_SECTORS (DDHDMULT * 11)

#undef DEBUG_DRIVE_ID

/* UAE-1ADF (ADF_EXT2)
* W reserved
* W number of tracks (default 2*80=160)
*
* W reserved
* W type, 0=normal AmigaDOS track, 1 = raw MFM (upper byte = disk revolutions - 1)
* L available space for track in bytes (must be even)
* L track length in bits
*/

static int side, direction;
static uae_u8 selected = 15, disabled;

static uae_u8 writebuffer[544 * MAX_SECTORS];

#define DISK_INDEXSYNC 1
#define DISK_WORDSYNC 2
#define DISK_REVOLUTION 4 /* 8,16,32,64 */

#define DSKREADY_TIME 4
#define DSKREADY_DOWN_TIME 10

static int diskevent_flag;
static int disk_sync_cycle;

#if 0
#define MAX_DISK_WORDS_PER_LINE 50 /* depends on floppy_speed */
static uae_u32 dma_tab[MAX_DISK_WORDS_PER_LINE + 1];
#endif
static int dskdmaen, dsklength, dsklength2, dsklen;
static uae_u16 dskbytr_val;
static uae_u32 dskpt;
static int dma_enable, bitoffset, syncoffset;
static uae_u16 word, dsksync;
/* Always carried through to the next line.  */
static int disk_hpos;
static int disk_jitter;

typedef enum { TRACK_AMIGADOS, TRACK_RAW, TRACK_RAW1, TRACK_PCDOS, TRACK_DISKSPARE, TRACK_NONE } image_tracktype;
typedef struct {
	uae_u16 len;
	uae_u32 offs;
	int bitlen, track;
	uae_u16 sync;
	image_tracktype type;
	int revolutions;
} trackid;

#define MAX_TRACKS (2 * 83)

/* We have three kinds of Amiga floppy drives
* - internal A500/A2000 drive:
*   ID is always DRIVE_ID_NONE (S.T.A.G expects this)
* - HD drive (A3000/A4000):
*   ID is DRIVE_ID_35DD if DD floppy is inserted or drive is empty
*   ID is DRIVE_ID_35HD if HD floppy is inserted
* - regular external drive:
*   ID is always DRIVE_ID_35DD
*/

#define DRIVE_ID_NONE  0x00000000
#define DRIVE_ID_35DD  0xFFFFFFFF
#define DRIVE_ID_35HD  0xAAAAAAAA
#define DRIVE_ID_525SD 0x55555555 /* 40 track 5.25 drive , kickstart does not recognize this */

typedef enum { ADF_NONE = -1, ADF_NORMAL, ADF_EXT1, ADF_EXT2, ADF_FDI, ADF_IPF, ADF_CATWEASEL, ADF_PCDOS } drive_filetype;
typedef struct {
	struct zfile *diskfile;
	struct zfile *writediskfile;
	drive_filetype filetype;
	trackid trackdata[MAX_TRACKS];
	trackid writetrackdata[MAX_TRACKS];
	int buffered_cyl, buffered_side;
	int cyl;
	int motoroff;
	int motordelay; /* dskrdy needs some clock cycles before it changes after switching off motor */
	int state;
	int wrprot;
	uae_u16 bigmfmbuf[0x4000 * DDHDMULT];
	uae_u16 tracktiming[0x4000 * DDHDMULT];
	int multi_revolution;
	int skipoffset;
	int mfmpos;
	int indexoffset;
	int tracklen;
	int revolutions;
	int prevtracklen;
	int trackspeed;
	int dmalen;
	int num_tracks, write_num_tracks, num_secs;
	int hard_num_cyls;
	int dskchange;
	int dskchange_time;
	int dskready;
	int dskready_time;
	int dskready_down_time;
	int writtento;
	int steplimit;
	frame_time_t steplimitcycle;
	int indexhack, indexhackmode;
	int ddhd; /* 1=DD 2=HD */
	int drive_id_scnt; /* drive id shift counter */
	int idbit;
	unsigned long drive_id; /* drive id to be reported */
	TCHAR newname[256]; /* storage space for new filename during eject delay */
	uae_u32 crc32;
#ifdef FDI2RAW
	FDI *fdi;
#endif
	int useturbo;
	int floppybitcounter; /* number of bits left */
#ifdef CATWEASEL
	catweasel_drive *catweasel;
#else
	int catweasel;
	int amax;
#endif
} drive;

int disk_debug_logging;
int disk_debug_mode;
int disk_debug_track = -1;

#define MIN_STEPLIMIT_CYCLE (CYCLE_UNIT * 250)

static uae_u16 bigmfmbufw[0x4000 * DDHDMULT];
static drive floppy[MAX_FLOPPY_DRIVES];
static TCHAR dfxhistory[2][MAX_PREVIOUS_FLOPPIES][MAX_DPATH];

static uae_u8 exeheader[]={0x00,0x00,0x03,0xf3,0x00,0x00,0x00,0x00};
static uae_u8 bootblock[]={
	0x44,0x4f,0x53,0x00,0xc0,0x20,0x0f,0x19,0x00,0x00,0x03,0x70,0x43,0xfa,0x00,0x18,
	0x4e,0xae,0xff,0xa0,0x4a,0x80,0x67,0x0a,0x20,0x40,0x20,0x68,0x00,0x16,0x70,0x00,
	0x4e,0x75,0x70,0xff,0x60,0xfa,0x64,0x6f,0x73,0x2e,0x6c,0x69,0x62,0x72,0x61,0x72,
	0x79
};

#define FS_OFS_DATABLOCKSIZE 488
#define FS_FLOPPY_BLOCKSIZE 512
#define FS_EXTENSION_BLOCKS 72
#define FS_FLOPPY_TOTALBLOCKS 1760
#define FS_FLOPPY_RESERVED 2

static void writeimageblock (struct zfile *dst, uae_u8 *sector, int offset)
{
	zfile_fseek (dst, offset, SEEK_SET);
	zfile_fwrite (sector, FS_FLOPPY_BLOCKSIZE, 1, dst);
}

static void disk_checksum(uae_u8 *p, uae_u8 *c)
{
	uae_u32 cs = 0;
	int i;
	for (i = 0; i < FS_FLOPPY_BLOCKSIZE; i+= 4)
		cs += (p[i] << 24) | (p[i+1] << 16) | (p[i+2] << 8) | (p[i+3] << 0);
	cs = -cs;
	c[0] = cs >> 24; c[1] = cs >> 16; c[2] = cs >> 8; c[3] = cs >> 0;
}

static int dirhash (const uae_char *name)
{
	unsigned long hash;
	int i;

	hash = strlen (name);
	for(i = 0; i < strlen (name); i++) {
		hash = hash * 13;
		hash = hash + toupper (name[i]);
		hash = hash & 0x7ff;
	}
	hash = hash % ((FS_FLOPPY_BLOCKSIZE / 4) - 56);
	return hash;
}

static void disk_date (uae_u8 *p)
{
	time_t t;
	struct tm *today;
	int year, days, minutes, ticks;
	TCHAR tmp[10];

	time (&t);
	today = localtime( &t );
	_tcsftime (tmp, sizeof (tmp) / sizeof (TCHAR), L"%Y", today);
	year = _tstoi (tmp);
	_tcsftime (tmp, sizeof (tmp) / sizeof (TCHAR), L"%j", today);
	days = _tstoi (tmp) - 1;
	_tcsftime (tmp, sizeof (tmp) / sizeof (TCHAR), L"%H", today);
	minutes = _tstoi (tmp) * 60;
	_tcsftime (tmp, sizeof (tmp) / sizeof (TCHAR), L"%M", today);
	minutes += _tstoi (tmp);
	_tcsftime (tmp, sizeof (tmp) / sizeof (TCHAR), L"%S", today);
	ticks = _tstoi (tmp) * 50;
	while (year > 1978) {
		if ( !(year % 100) ? !(year % 400) : !(year % 4) ) days++;
		days += 365;
		year--;
	}
	p[0] = days >> 24; p[1] = days >> 16; p[2] = days >> 8; p[3] = days >> 0;
	p[4] = minutes >> 24; p[5] = minutes >> 16; p[6] = minutes >> 8; p[7] = minutes >> 0;
	p[8] = ticks >> 24; p[9] = ticks >> 16; p[10] = ticks >> 8; p[11] = ticks >> 0;
}

static void createbootblock (uae_u8 *sector, int bootable)
{
	memset (sector, 0, FS_FLOPPY_BLOCKSIZE);
	memcpy (sector, "DOS", 3);
	if (bootable)
		memcpy (sector, bootblock, sizeof (bootblock));
}

static void createrootblock (uae_u8 *sector, char *disk_name)
{
	memset (sector, 0, FS_FLOPPY_BLOCKSIZE);
	sector[0+3] = 2;
	sector[12+3] = 0x48;
	sector[312] = sector[313] = sector[314] = sector[315] = (uae_u8)0xff;
	sector[316+2] = 881 >> 8; sector[316+3] = 881 & 255;
	sector[432] = strlen (disk_name);
	strcpy ((char*)sector + 433, disk_name);
	sector[508 + 3] = 1;
	disk_date (sector + 420);
	memcpy (sector + 472, sector + 420, 3 * 4);
	memcpy (sector + 484, sector + 420, 3 * 4);
}

static int getblock (uae_u8 *bitmap)
{
	int i = 0;
	while (bitmap[i] != 0xff) {
		if (bitmap[i] == 0) {
			bitmap[i] = 1;
			return i;
		}
		i++;
	}
	return -1;
}

static void pl (uae_u8 *sector, int offset, uae_u32 v)
{
	sector[offset + 0] = v >> 24;
	sector[offset + 1] = v >> 16;
	sector[offset + 2] = v >> 8;
	sector[offset + 3] = v >> 0;
}

static int createdirheaderblock (uae_u8 *sector, int parent, const char *filename, uae_u8 *bitmap)
{
	int block = getblock (bitmap);

	memset (sector, 0, FS_FLOPPY_BLOCKSIZE);
	pl (sector, 0, 2);
	pl (sector, 4, block);
	disk_date (sector + 512 - 92);
	sector[512 - 80] = strlen (filename);
	strcpy ((char*)sector + 512 - 79, filename);
	pl (sector, 512 - 12, parent);
	pl (sector, 512 - 4, 2);
	return block;
}

static int createfileheaderblock (struct zfile *z,uae_u8 *sector, int parent, const char *filename, struct zfile *src, uae_u8 *bitmap)
{
	uae_u8 sector2[FS_FLOPPY_BLOCKSIZE];
	uae_u8 sector3[FS_FLOPPY_BLOCKSIZE];
	int block = getblock (bitmap);
	int datablock = getblock (bitmap);
	int datasec = 1;
	int extensions;
	int extensionblock, extensioncounter, headerextension = 1;
	int size;

	zfile_fseek (src, 0, SEEK_END);
	size = zfile_ftell (src);
	zfile_fseek (src, 0, SEEK_SET);
	extensions = (size + FS_OFS_DATABLOCKSIZE - 1) / FS_OFS_DATABLOCKSIZE;

	memset (sector, 0, FS_FLOPPY_BLOCKSIZE);
	pl (sector, 0, 2);
	pl (sector, 4, block);
	pl (sector, 8, extensions > FS_EXTENSION_BLOCKS ? FS_EXTENSION_BLOCKS : extensions);
	pl (sector, 16, datablock);
	pl (sector, FS_FLOPPY_BLOCKSIZE - 188, size);
	disk_date (sector + FS_FLOPPY_BLOCKSIZE - 92);
	sector[FS_FLOPPY_BLOCKSIZE - 80] = strlen (filename);
	strcpy ((char*)sector + FS_FLOPPY_BLOCKSIZE - 79, filename);
	pl (sector, FS_FLOPPY_BLOCKSIZE - 12, parent);
	pl (sector, FS_FLOPPY_BLOCKSIZE - 4, -3);
	extensioncounter = 0;
	extensionblock = 0;

	while (size > 0) {
		int datablock2 = datablock;
		int extensionblock2 = extensionblock;
		if (extensioncounter == FS_EXTENSION_BLOCKS) {
			extensioncounter = 0;
			extensionblock = getblock (bitmap);
			if (datasec > FS_EXTENSION_BLOCKS + 1) {
				pl (sector3, 8, FS_EXTENSION_BLOCKS);
				pl (sector3, FS_FLOPPY_BLOCKSIZE - 8, extensionblock);
				pl (sector3, 4, extensionblock2);
				disk_checksum(sector3, sector3 + 20);
				writeimageblock (z, sector3, extensionblock2 * FS_FLOPPY_BLOCKSIZE);
			} else {
				pl (sector, 512 - 8, extensionblock);
			}
			memset (sector3, 0, FS_FLOPPY_BLOCKSIZE);
			pl (sector3, 0, 16);
			pl (sector3, FS_FLOPPY_BLOCKSIZE - 12, block);
			pl (sector3, FS_FLOPPY_BLOCKSIZE - 4, -3);
		}
		memset (sector2, 0, FS_FLOPPY_BLOCKSIZE);
		pl (sector2, 0, 8);
		pl (sector2, 4, block);
		pl (sector2, 8, datasec++);
		pl (sector2, 12, size > FS_OFS_DATABLOCKSIZE ? FS_OFS_DATABLOCKSIZE : size);
		zfile_fread (sector2 + 24, size > FS_OFS_DATABLOCKSIZE ? FS_OFS_DATABLOCKSIZE : size, 1, src);
		size -= FS_OFS_DATABLOCKSIZE;
		datablock = 0;
		if (size > 0) datablock = getblock (bitmap);
		pl (sector2, 16, datablock);
		disk_checksum(sector2, sector2 + 20);
		writeimageblock (z, sector2, datablock2 * FS_FLOPPY_BLOCKSIZE);
		if (datasec <= FS_EXTENSION_BLOCKS + 1)
			pl (sector, 512 - 204 - extensioncounter * 4, datablock2);
		else
			pl (sector3, 512 - 204 - extensioncounter * 4, datablock2);
		extensioncounter++;
	}
	if (datasec > FS_EXTENSION_BLOCKS) {
		pl (sector3, 8, extensioncounter);
		disk_checksum(sector3, sector3 + 20);
		writeimageblock (z, sector3, extensionblock * FS_FLOPPY_BLOCKSIZE);
	}
	disk_checksum(sector, sector + 20);
	writeimageblock (z, sector, block * FS_FLOPPY_BLOCKSIZE);
	return block;
}

static void createbitmapblock (uae_u8 *sector, uae_u8 *bitmap)
{
	uae_u8 mask;
	int i, j;
	memset (sector, 0, FS_FLOPPY_BLOCKSIZE);
	for (i = FS_FLOPPY_RESERVED; i < FS_FLOPPY_TOTALBLOCKS; i += 8) {
		mask = 0;
		for (j = 0; j < 8; j++) {
			if (bitmap[i + j]) mask |= 1 << j;
		}
		sector[4 + i / 8] = mask;
	}
	disk_checksum(sector, sector + 0);
}

static int createimagefromexe (struct zfile *src, struct zfile *dst)
{
	uae_u8 sector1[FS_FLOPPY_BLOCKSIZE], sector2[FS_FLOPPY_BLOCKSIZE];
	uae_u8 bitmap[FS_FLOPPY_TOTALBLOCKS + 8];
	int exesize;
	int blocksize = FS_OFS_DATABLOCKSIZE;
	int blocks, extensionblocks;
	int totalblocks;
	int fblock1, dblock1;
	char *fname1 = "runme.exe";
	TCHAR *fname1b = L"runme.adf";
	char *fname2 = "startup-sequence";
	char *dirname1 = "s";
	struct zfile *ss;

	memset (bitmap, 0, sizeof (bitmap));
	zfile_fseek (src, 0, SEEK_END);
	exesize = zfile_ftell (src);
	blocks = (exesize + blocksize - 1) / blocksize;
	extensionblocks = (blocks + FS_EXTENSION_BLOCKS - 1) / FS_EXTENSION_BLOCKS;
	/* bootblock=2, root=1, bitmap=1, startup-sequence=1+1, exefileheader=1 */
	totalblocks = 2 + 1 + 1 + 2 + 1 + blocks + extensionblocks;
	if (totalblocks > FS_FLOPPY_TOTALBLOCKS)
		return 0;

	bitmap[880] = 1;
	bitmap[881] = 1;
	bitmap[0] = 1;
	bitmap[1] = 1;

	dblock1 = createdirheaderblock (sector2, 880, dirname1, bitmap);
	ss = zfile_fopen_empty (src, fname1b, strlen (fname1));
	zfile_fwrite (fname1, strlen(fname1), 1, ss);
	fblock1 = createfileheaderblock (dst, sector1,  dblock1, fname2, ss, bitmap);
	zfile_fclose (ss);
	pl (sector2, 24 + dirhash (fname2) * 4, fblock1);
	disk_checksum(sector2, sector2 + 20);
	writeimageblock (dst, sector2, dblock1 * FS_FLOPPY_BLOCKSIZE);

	fblock1 = createfileheaderblock (dst, sector1, 880, fname1, src, bitmap);

	createrootblock (sector1, "empty");
	pl (sector1, 24 + dirhash (fname1) * 4, fblock1);
	pl (sector1, 24 + dirhash (dirname1) * 4, dblock1);
	disk_checksum(sector1, sector1 + 20);
	writeimageblock (dst, sector1, 880 * FS_FLOPPY_BLOCKSIZE);

	createbitmapblock (sector1, bitmap);
	writeimageblock (dst, sector1, 881 * FS_FLOPPY_BLOCKSIZE);

	createbootblock (sector1, 1);
	writeimageblock (dst, sector1, 0 * FS_FLOPPY_BLOCKSIZE);

	return 1;
}

static int get_floppy_speed (void)
{
	int m = currprefs.floppy_speed;
	if (m <= 10)
		m = 100;
	m = NORMAL_FLOPPY_SPEED * 100 / m;
	return m;
}

static int get_floppy_speed2 (drive *drv)
{
	int m = get_floppy_speed () * drv->tracklen / (2 * 8 * FLOPPY_WRITE_LEN * drv->ddhd);
	if (m <= 0)
		m = 1;
	return m;
}

static TCHAR *drive_id_name(drive *drv)
{
	switch(drv->drive_id)
	{
	case DRIVE_ID_35HD : return L"3.5HD";
	case DRIVE_ID_525SD: return L"5.25SD";
	case DRIVE_ID_35DD : return L"3.5DD";
	case DRIVE_ID_NONE : return L"NONE";
	}
	return L"UNKNOWN";
}

/* Simulate exact behaviour of an A3000T 3.5 HD disk drive.
* The drive reports to be a 3.5 DD drive whenever there is no
* disk or a 3.5 DD disk is inserted. Only 3.5 HD drive id is reported
* when a real 3.5 HD disk is inserted. -Adil
*/
static void drive_settype_id (drive *drv)
{
	int t = currprefs.dfxtype[drv - &floppy[0]];

	switch (t)
	{
	case DRV_35_HD:
#ifdef FLOPPY_DRIVE_HD
		if (!drv->diskfile || drv->ddhd <= 1)
			drv->drive_id = DRIVE_ID_35DD;
		else
			drv->drive_id = DRIVE_ID_35HD;
#else
		drv->drive_id = DRIVE_ID_35DD;
#endif
		break;
	case DRV_35_DD_ESCOM:
	case DRV_35_DD:
	default:
		drv->drive_id = DRIVE_ID_35DD;
		break;
	case DRV_525_SD:
		drv->drive_id = DRIVE_ID_525SD;
		break;
	case DRV_NONE:
		drv->drive_id = DRIVE_ID_NONE;
		break;
	}
#ifdef DEBUG_DRIVE_ID
	write_log (L"drive_settype_id: DF%d: set to %s\n", drv-floppy, drive_id_name(drv));
#endif
}

static void drive_image_free (drive *drv)
{
	switch (drv->filetype)
	{
	case ADF_IPF:
#ifdef CAPS
		caps_unloadimage (drv - floppy);
#endif
		break;
	case ADF_FDI:
#ifdef FDI2RAW
		fdi2raw_header_free (drv->fdi);
		drv->fdi = 0;
#endif
		break;
	}
	drv->filetype = ADF_NONE;
	zfile_fclose (drv->diskfile);
	drv->diskfile = 0;
	zfile_fclose (drv->writediskfile);
	drv->writediskfile = 0;
}

static int drive_insert (drive * drv, struct uae_prefs *p, int dnum, const TCHAR *fname);

static void reset_drive_gui (int i)
{
	gui_data.drive_disabled[i] = 0;
	gui_data.df[i][0] = 0;
	gui_data.crc32[i] = 0;
	if (currprefs.dfxtype[i] < 0)
		gui_data.drive_disabled[i] = 1;
}

static void setamax (void)
{
#ifdef AMAX
	if (currprefs.amaxromfile[0]) {
		/* Put A-Max as last drive in drive chain */
		int j;
		for (j = 0; j < MAX_FLOPPY_DRIVES; j++)
			if (floppy[j].amax)
				return;
		for (j = 0; j < MAX_FLOPPY_DRIVES; j++) {
			if ((1 << j) & disabled) {
				floppy[j].amax = 1;
				write_log (L"AMAX: drive %d\n", j);
				return;
			}
		}
	}
#endif
}

static void reset_drive (int i)
{
	drive *drv = &floppy[i];

	drv->amax = 0;
	drive_image_free (drv);
	drv->motoroff = 1;
	disabled &= ~(1 << i);
	if (currprefs.dfxtype[i] < 0)
		disabled |= 1 << i;
	reset_drive_gui(i);
	/* most internal Amiga floppy drives won't enable
	* diskready until motor is running at full speed
	* and next indexsync has been passed
	*/
	drv->indexhackmode = 0;
	if (i == 0 && currprefs.dfxtype[i] == 0)
		drv->indexhackmode = 1;
	drv->dskchange_time = 0;
	drv->buffered_cyl = -1;
	drv->buffered_side = -1;
	gui_led (i + LED_DF0, 0);
	drive_settype_id (drv);
	_tcscpy (currprefs.df[i], changed_prefs.df[i]);
	drv->newname[0] = 0;
	if (!drive_insert (drv, &currprefs, i, currprefs.df[i]))
		disk_eject (i);
}

/* code for track display */
static void update_drive_gui (int num)
{
	drive *drv = floppy + num;
	int writ = dskdmaen == 3 && drv->state && !(selected & (1 << num));

	if (drv->state == gui_data.drive_motor[num]
	&& drv->cyl == gui_data.drive_track[num]
	&& side == gui_data.drive_side
		&& drv->crc32 == gui_data.crc32[num]
	&& writ == gui_data.drive_writing[num]
	&& !_tcscmp (gui_data.df[num], currprefs.df[num]))
		return;
	_tcscpy (gui_data.df[num], currprefs.df[num]);
	gui_data.crc32[num] = drv->crc32;
	gui_data.drive_motor[num] = drv->state;
	gui_data.drive_track[num] = drv->cyl;
	gui_data.drive_side = side;
	gui_data.drive_writing[num] = writ;
	gui_led (num + LED_DF0, (gui_data.drive_motor[num] ? 1 : 0) | (gui_data.drive_writing[num] ? 2 : 0));
}

static void drive_fill_bigbuf (drive * drv,int);

int DISK_validate_filename (const TCHAR *fname, int leave_open, int *wrprot, uae_u32 *crc32, struct zfile **zf)
{
	if (zf)
		*zf = NULL;
	if (crc32)
		*crc32 = 0;
	if (leave_open) {
		struct zfile *f = zfile_fopen (fname, L"r+b", ZFD_NORMAL | ZFD_DISKHISTORY);
		if (f) {
			if (wrprot)
				*wrprot = 0;
		} else {
			if (wrprot)
				*wrprot = 1;
			f = zfile_fopen (fname, L"rb", ZFD_NORMAL | ZFD_DISKHISTORY);
		}
		if (f && crc32)
			*crc32 = zfile_crc32 (f);
		if (!zf)
			zfile_fclose (f);
		else
			*zf = f;
		return f ? 1 : 0;
	} else {
		if (zfile_exists (fname)) {
			if (wrprot)
				*wrprot = 0;
			if (crc32) {
				struct zfile *f = zfile_fopen (fname, L"rb", ZFD_NORMAL | ZFD_DISKHISTORY);
				if (f)
					*crc32 = zfile_crc32 (f);
				zfile_fclose (f);
			}
			return 1;
		} else {
			if (wrprot)
				*wrprot = 1;
			return 0;
		}
	}
}

static void updatemfmpos (drive *drv)
{
	if (drv->prevtracklen)
		drv->mfmpos = drv->mfmpos * (drv->tracklen * 1000 / drv->prevtracklen) / 1000;
	drv->mfmpos %= drv->tracklen;
	drv->prevtracklen = drv->tracklen;
}

static void track_reset (drive *drv)
{
	drv->tracklen = FLOPPY_WRITE_LEN * drv->ddhd * 2 * 8;
	drv->revolutions = 1;
	drv->trackspeed = get_floppy_speed ();
	drv->buffered_side = -1;
	drv->skipoffset = -1;
	drv->tracktiming[0] = 0;
	memset (drv->bigmfmbuf, 0xaa, FLOPPY_WRITE_LEN * 2 * drv->ddhd);
	updatemfmpos (drv);
}

static int read_header_ext2 (struct zfile *diskfile, trackid *trackdata, int *num_tracks, int *ddhd)
{
	uae_u8 buffer[2 + 2 + 4 + 4];
	trackid *tid;
	int offs;
	int i;

	zfile_fseek (diskfile, 0, SEEK_SET);
	zfile_fread (buffer, 1, 8, diskfile);
	if (strncmp ((char*)buffer, "UAE-1ADF", 8))
		return 0;
	zfile_fread (buffer, 1, 4, diskfile);
	*num_tracks = buffer[2] * 256 + buffer[3];
	offs = 8 + 2 + 2 + (*num_tracks) * (2 + 2 + 4 + 4);

	for (i = 0; i < (*num_tracks); i++) {
		tid = trackdata + i;
		zfile_fread (buffer, 2 + 2 + 4 + 4, 1, diskfile);
		tid->type = (image_tracktype)buffer[3];
		tid->revolutions = buffer[2] + 1;
		tid->len = buffer[5] * 65536 + buffer[6] * 256 + buffer[7];
		tid->bitlen = buffer[9] * 65536 + buffer[10] * 256 + buffer[11];
		tid->offs = offs;
		if (tid->len > 20000 && ddhd)
			*ddhd = 2;
		tid->track = i;
		offs += tid->len;
	}
	return 1;
}

TCHAR *DISK_get_saveimagepath (const TCHAR *name)
{
	static TCHAR name1[MAX_DPATH];
	TCHAR name2[MAX_DPATH];
	TCHAR path[MAX_DPATH];
	int i;

	_tcscpy (name2, name);
	i = _tcslen (name2) - 1;
	while (i > 0) {
		if (name2[i] == '.') {
			name2[i] = 0;
			break;
		}
		i--;
	}
	while (i > 0) {
		if (name2[i] == '/' || name2[i] == '\\') {
			i++;
			break;
		}
		i--;
	}
	fetch_saveimagepath (path, sizeof (path), 1);
	_stprintf (name1, L"%s%s_save.adf", path, name2 + i);
	return name1;
}

static struct zfile *getwritefile (const TCHAR *name, int *wrprot)
{
	struct zfile *zf;
	DISK_validate_filename (DISK_get_saveimagepath (name), 1, wrprot, NULL, &zf);
	return zf;
}

static int iswritefileempty (const TCHAR *name)
{
	struct zfile *zf;
	int wrprot;
	uae_char buffer[8];
	trackid td[MAX_TRACKS];
	int tracks, ddhd, i, ret;

	zf = getwritefile (name, &wrprot);
	if (!zf) return 1;
	zfile_fread (buffer, sizeof (char), 8, zf);
	if (strncmp ((uae_char*)buffer, "UAE-1ADF", 8))
		return 0;
	ret = read_header_ext2 (zf, td, &tracks, &ddhd);
	zfile_fclose (zf);
	if (!ret)
		return 1;
	for (i = 0; i < tracks; i++) {
		if (td[i].bitlen) return 0;
	}
	return 1;
}

static int openwritefile (drive *drv, int create)
{
	int wrprot = 0;

	drv->writediskfile = getwritefile (currprefs.df[drv - &floppy[0]], &wrprot);
	if (drv->writediskfile) {
		drv->wrprot = wrprot;
		if (!read_header_ext2 (drv->writediskfile, drv->writetrackdata, &drv->write_num_tracks, 0)) {
			zfile_fclose (drv->writediskfile);
			drv->writediskfile = 0;
			drv->wrprot = 1;
		} else {
			if (drv->write_num_tracks > drv->num_tracks)
				drv->num_tracks = drv->write_num_tracks;
		}
	} else if (zfile_iscompressed (drv->diskfile)) {
		drv->wrprot = 1;
	}
	return drv->writediskfile ? 1 : 0;
}

static int diskfile_iswriteprotect (const TCHAR *fname, int *needwritefile, drive_type *drvtype)
{
	struct zfile *zf1, *zf2;
	int wrprot1 = 0, wrprot2 = 1;
	uae_char buffer[25];

	*needwritefile = 0;
	*drvtype = DRV_35_DD;
	DISK_validate_filename (fname, 1, &wrprot1, NULL, &zf1);
	if (!zf1)
		return 1;
	if (zfile_iscompressed (zf1)) {
		wrprot1 = 1;
		*needwritefile = 1;
	}
	zf2 = getwritefile (fname, &wrprot2);
	zfile_fclose (zf2);
	zfile_fread (buffer, sizeof (char), 25, zf1);
	zfile_fclose (zf1);
	if (strncmp ((uae_char*) buffer, "CAPS", 4) == 0) {
		*needwritefile = 1;
		return wrprot2;
	}
	if (strncmp ((uae_char*) buffer, "Formatted Disk Image file", 25) == 0) {
		*needwritefile = 1;
		return wrprot2;
	}
	if (strncmp ((uae_char*) buffer, "UAE-1ADF", 8) == 0) {
		if (wrprot1)
			return wrprot2;
		return wrprot1;
	}
	if (strncmp ((uae_char*) buffer, "UAE--ADF", 8) == 0) {
		*needwritefile = 1;
		return wrprot2;
	}
	if (memcmp (exeheader, buffer, sizeof (exeheader)) == 0)
		return 0;
	if (wrprot1)
		return wrprot2;
	return wrprot1;
}

static int drive_insert (drive * drv, struct uae_prefs *p, int dnum, const TCHAR *fname)
{
	uae_u8 buffer[2 + 2 + 4 + 4];
	trackid *tid;
	int num_tracks, size;
	int canauto;
	const TCHAR *ext;

	gui_disk_image_change (dnum, fname);
	drive_image_free (drv);
	DISK_validate_filename (fname, 1, &drv->wrprot, &drv->crc32, &drv->diskfile);
	drv->ddhd = 1;
	drv->num_secs = 0;
	drv->hard_num_cyls = p->dfxtype[dnum] == DRV_525_SD ? 40 : 80;
	drv->tracktiming[0] = 0;
	drv->useturbo = 0;
	drv->indexoffset = 0;

	canauto = 0;
	ext = _tcsrchr (fname, '.');
	if (ext) {
		if (!_tcsicmp (ext + 1, L"adf") || !_tcsicmp (ext + 1, L"adz") || !_tcsicmp (ext + 1, L"st") || !_tcsicmp (ext + 1, L"ima") || !_tcsicmp (ext + 1, L"img"))
			canauto = 1;
	}

	if (!drv->motoroff) {
		drv->dskready_time = DSKREADY_TIME;
		drv->dskready_down_time = 0;
	}

	if (drv->diskfile == 0 && !drv->catweasel) {
		track_reset (drv);
		return 0;
	}

	if (input_recording > 0) {
		inprec_rstart (INPREC_DISKINSERT);
		inprec_ru8 (dnum);
		inprec_rstr (fname);
		inprec_rend ();
	}

	_tcsncpy (currprefs.df[dnum], fname, 255);
	currprefs.df[dnum][255] = 0;
	_tcsncpy (changed_prefs.df[dnum], fname, 255);
	changed_prefs.df[dnum][255] = 0;
	_tcscpy (drv->newname, fname);
	gui_filename (dnum, fname);

	memset (buffer, 0, sizeof buffer);
	size = 0;
	if (drv->diskfile) {
		zfile_fread (buffer, sizeof (char), 8, drv->diskfile);
		zfile_fseek (drv->diskfile, 0, SEEK_END);
		size = zfile_ftell (drv->diskfile);
		zfile_fseek (drv->diskfile, 0, SEEK_SET);
	}

	if (drv->catweasel) {

		drv->wrprot = 1;
		drv->filetype = ADF_CATWEASEL;
		drv->num_tracks = 80;
		drv->ddhd = 1;

#ifdef CAPS
	} else if (strncmp ((char*)buffer, "CAPS", 4) == 0) {

		drv->wrprot = 1;
		if (!caps_loadimage (drv->diskfile, drv - floppy, &num_tracks)) {
			zfile_fclose (drv->diskfile);
			drv->diskfile = 0;
			return 0;
		}
		drv->num_tracks = num_tracks;
		drv->filetype = ADF_IPF;
#endif
#ifdef FDI2RAW
	} else if (drv->fdi = fdi2raw_header (drv->diskfile)) {

		drv->wrprot = 1;
		drv->num_tracks = fdi2raw_get_last_track (drv->fdi);
		drv->num_secs = fdi2raw_get_num_sector (drv->fdi);
		drv->filetype = ADF_FDI;
#endif
	} else if (strncmp ((char*)buffer, "UAE-1ADF", 8) == 0) {

		read_header_ext2 (drv->diskfile, drv->trackdata, &drv->num_tracks, &drv->ddhd);
		drv->filetype = ADF_EXT2;
		drv->num_secs = 11;
		if (drv->ddhd > 1)
			drv->num_secs = 22;

	} else if (strncmp ((char*)buffer, "UAE--ADF", 8) == 0) {
		int offs = 160 * 4 + 8;
		int i;

		drv->wrprot = 1;
		drv->filetype = ADF_EXT1;
		drv->num_tracks = 160;
		drv->num_secs = 11;

		zfile_fseek (drv->diskfile, 8, SEEK_SET);
		for (i = 0; i < 160; i++) {
			tid = &drv->trackdata[i];
			zfile_fread (buffer, 4, 1, drv->diskfile);
			tid->sync = buffer[0] * 256 + buffer[1];
			tid->len = buffer[2] * 256 + buffer[3];
			tid->offs = offs;
			tid->revolutions = 1;
			if (tid->sync == 0) {
				tid->type = TRACK_AMIGADOS;
				tid->bitlen = 0;
			} else {
				tid->type = TRACK_RAW1;
				tid->bitlen = tid->len * 8;
			}
			offs += tid->len;
		}

	} else if (memcmp (exeheader, buffer, sizeof (exeheader)) == 0) {
		int i;
		struct zfile *z = zfile_fopen_empty (NULL, L"", 512 * 1760);
		createimagefromexe (drv->diskfile, z);
		drv->filetype = ADF_NORMAL;
		zfile_fclose (drv->diskfile);
		drv->diskfile = z;
		drv->num_tracks = 160;
		drv->num_secs = 11;
		for (i = 0; i < drv->num_tracks; i++) {
			tid = &drv->trackdata[i];
			tid->type = TRACK_AMIGADOS;
			tid->len = 512 * drv->num_secs;
			tid->bitlen = 0;
			tid->offs = i * 512 * drv->num_secs;
			tid->revolutions = 1;
		}
		drv->useturbo = 1;

	} else if (canauto && (

		// double sided
		size == 9 * 80 * 2 * 512 || size == 18 * 80 * 2 * 512 || size == 10 * 80 * 2 * 512 || size == 20 * 80 * 2 * 512 ||
		size == 9 * 81 * 2 * 512 || size == 18 * 81 * 2 * 512 || size == 10 * 81 * 2 * 512 || size == 20 * 81 * 2 * 512 ||
		size == 9 * 82 * 2 * 512 || size == 18 * 82 * 2 * 512 || size == 10 * 82 * 2 * 512 || size == 20 * 82 * 2 * 512 ||
		// single sided
		size == 9 * 80 * 1 * 512 || size == 18 * 80 * 1 * 512 || size == 10 * 80 * 1 * 512 || size == 20 * 80 * 1 * 512 ||
		size == 9 * 81 * 1 * 512 || size == 18 * 81 * 1 * 512 || size == 10 * 81 * 1 * 512 || size == 20 * 81 * 1 * 512 ||
		size == 9 * 82 * 1 * 512 || size == 18 * 82 * 1 * 512 || size == 10 * 82 * 1 * 512 || size == 20 * 82 * 1 * 512)) {
			/* PC formatted image */
			int i, side;

			for (side = 2; side > 0; side--) {
				if (       size ==  9 * 80 * side * 512 || size ==  9 * 81 * side * 512 || size ==  9 * 82 * side * 512) {
					drv->num_secs = 9;
					drv->ddhd = 1;
					break;
				} else if (size == 18 * 80 * side * 512 || size == 18 * 81 * side * 512 || size == 18 * 82 * side * 512) {
					drv->num_secs = 18;
					drv->ddhd = 2;
					break;
				} else if (size == 10 * 80 * side * 512 || size == 10 * 81 * side * 512 || size == 10 * 82 * side * 512) {
					drv->num_secs = 10;
					drv->ddhd = 1;
					break;
				} else if (size == 20 * 80 * side * 512 || size == 20 * 81 * side * 512 || size == 20 * 82 * side * 512) {
					drv->num_secs = 20;
					drv->ddhd = 2;
					break;
				}
			}
			drv->num_tracks = size / (drv->num_secs * 512);

			drv->filetype = ADF_PCDOS;
			tid = &drv->trackdata[0];
			for (i = 0; i < drv->num_tracks; i++) {
				tid->type = TRACK_PCDOS;
				tid->len = 512 * drv->num_secs;
				tid->bitlen = 0;
				tid->offs = i * 512 * drv->num_secs;
				if (side == 1) {
					tid++;
					tid->type = TRACK_NONE;
					tid->len = 512 * drv->num_secs;
				}
				tid->revolutions = 1;
				tid++;

			}
			if (side == 1)
				drv->num_tracks *= 2;

	} else {
		int i, ds;

		ds = 0;
		drv->filetype = ADF_NORMAL;

		/* High-density or diskspare disk? */
		drv->num_tracks = 0;
		if (size > 160 * 11 * 512) { // larger than standard adf?
			for (i = 80; i <= 83; i++) {
				if (size == i * 22 * 512 * 2) { // HD
					drv->ddhd = 2;
					drv->num_tracks = size / (512 * (drv->num_secs = 22));
					break;
				}
				if (size == i * 11 * 512 * 2) { // >80 cyl DD
					drv->num_tracks = size / (512 * (drv->num_secs = 11));
					break;
				}
				if (size == i * 12 * 512 * 2) { // ds 12 sectors
					drv->num_tracks = size / (512 * (drv->num_secs = 12));
					ds = 1;
					break;
				}
				if (size == i * 24 * 512 * 2) { // ds 24 sectors
					drv->num_tracks = size / (512 * (drv->num_secs = 24));
					drv->ddhd = 2;
					ds = 1;
					break;
				}
			}
			if (drv->num_tracks == 0) {
				drv->num_tracks = size / (512 * (drv->num_secs = 22));
				drv->ddhd = 2;
			}
		} else {
			drv->num_tracks = size / (512 * (drv->num_secs = 11));
		}

		if (!ds && drv->num_tracks > MAX_TRACKS)
			write_log (L"Your diskfile is too big, %d bytes!\n", size);
		for (i = 0; i < drv->num_tracks; i++) {
			tid = &drv->trackdata[i];
			tid->type = ds ? TRACK_DISKSPARE : TRACK_AMIGADOS;
			tid->len = 512 * drv->num_secs;
			tid->bitlen = 0;
			tid->offs = i * 512 * drv->num_secs;
			tid->revolutions = 1;
		}
	}
	openwritefile (drv, 0);
	drive_settype_id (drv); /* Set DD or HD drive */
	drive_fill_bigbuf (drv, 1);
	drv->mfmpos = uaerand ();
	drv->mfmpos |= (uaerand () << 16);
	drv->mfmpos %= drv->tracklen;
	drv->prevtracklen = 0;
#ifdef DRIVESOUND
	driveclick_insert (drv - floppy, 0);
#endif
	update_drive_gui (drv - floppy);
	return 1;
}

static void rand_shifter (drive *drv)
{
	int r = ((uaerand () >> 4) & 7) + 1;
	while (r-- > 0) {
		word <<= 1;
		word |= (uaerand () & 0x1000) ? 1 : 0;
		bitoffset++;
		bitoffset &= 15;
	}
}

static void set_steplimit (drive *drv)
{
	drv->steplimit = 10;
	drv->steplimitcycle = get_cycles ();
}

static int drive_empty (drive * drv)
{
#ifdef CATWEASEL
	if (drv->catweasel)
		return catweasel_disk_changed (drv->catweasel) == 0;
#endif
	return drv->diskfile == 0;
}

static void drive_step (drive * drv)
{
#ifdef CATWEASEL
	if (drv->catweasel) {
		int dir = direction ? -1 : 1;
		catweasel_step (drv->catweasel, dir);
		drv->cyl += dir;
		if (drv->cyl < 0)
			drv->cyl = 0;
		write_log (L"%d -> %d\n", dir, drv->cyl);
		return;
	}
#endif
	if (drv->steplimit && get_cycles() - drv->steplimitcycle < MIN_STEPLIMIT_CYCLE) {
		if (disk_debug_logging > 1)
			write_log (L" step ignored drive %d, %d",
			drv - floppy, (get_cycles() - drv->steplimitcycle) / CYCLE_UNIT);
		return;
	}
	/* A1200's floppy drive needs at least 30 raster lines between steps
	* but we'll use very small value for better compatibility with faster CPU emulation
	* (stupid trackloaders with CPU delay loops)
	*/
	set_steplimit (drv);
	if (!drive_empty (drv))
		drv->dskchange = 0;
	if (direction) {
		if (drv->cyl) {
			drv->cyl--;
#ifdef DRIVESOUND
			driveclick_click (drv - floppy, drv->cyl);
#endif
		}
		/*	else
		write_log (L"program tried to step beyond track zero\n");
		"no-click" programs does that
		*/
	} else {
		int maxtrack = drv->hard_num_cyls;
		if (drv->cyl < maxtrack + 3) {
			drv->cyl++;
#ifdef CATWEASEL
			if (drv->catweasel)
				catweasel_step (drv->catweasel, 1);
#endif
		}
		if (drv->cyl >= maxtrack)
			write_log (L"program tried to step over track %d\n", maxtrack);
#ifdef DRIVESOUND
		driveclick_click (drv - floppy, drv->cyl);
#endif
	}
	rand_shifter (drv);
	if (disk_debug_logging > 1)
		write_log (L" ->step %d", drv->cyl);
}

static int drive_track0 (drive * drv)
{
#ifdef CATWEASEL
	if (drv->catweasel)
		return catweasel_track0 (drv->catweasel);
#endif
	return drv->cyl == 0;
}

static int drive_writeprotected (drive * drv)
{
#ifdef CATWEASEL
	if (drv->catweasel)
		return 1;
#endif
	return drv->wrprot || drv->diskfile == NULL;
}

static int drive_running (drive * drv)
{
	return !drv->motoroff;
}

static void motordelay_func (uae_u32 v)
{
	floppy[v].motordelay = 0;
}

static void drive_motor (drive * drv, int off)
{
	if (drv->motoroff && !off) {
		drv->dskready_time = DSKREADY_TIME;
		rand_shifter (drv);
#ifdef DRIVESOUND
		driveclick_motor (drv - floppy, drv->dskready_down_time == 0 ? 2 : 1);
#endif
		if (disk_debug_logging > 1)
			write_log (L" ->motor on");
	}
	if (!drv->motoroff && off) {
		drv->drive_id_scnt = 0; /* Reset id shift reg counter */
		drv->dskready_down_time = DSKREADY_DOWN_TIME;
#ifdef DRIVESOUND
		driveclick_motor (drv - floppy, 0);
#endif
#ifdef DEBUG_DRIVE_ID
		write_log (L"drive_motor: Selected DF%d: reset id shift reg.\n",drv-floppy);
#endif
		if (disk_debug_logging > 1)
			write_log (L" ->motor off");
		if (currprefs.cpu_model <= 68010 && currprefs.m68k_speed == 0) {
			drv->motordelay = 1;
			event2_newevent2 (30, drv - floppy, motordelay_func);
		}
	}
	drv->motoroff = off;
	if (drv->motoroff) {
		drv->dskready = 0;
		drv->dskready_time = 0;
	}
#ifdef CATWEASEL
	if (drv->catweasel)
		catweasel_set_motor (drv->catweasel, !drv->motoroff);
#endif
}

static void read_floppy_data (struct zfile *diskfile, trackid *tid, int offset, uae_u8 *dst, int len)
{
	if (len == 0)
		return;
	zfile_fseek (diskfile, tid->offs + offset, SEEK_SET);
	zfile_fread (dst, 1, len, diskfile);
}

/* Megalomania does not like zero MFM words... */
static void mfmcode (uae_u16 * mfm, int words)
{
	uae_u32 lastword = 0;
	while (words--) {
		uae_u32 v = *mfm;
		uae_u32 lv = (lastword << 16) | v;
		uae_u32 nlv = 0x55555555 & ~lv;
		uae_u32 mfmbits = (nlv << 1) & (nlv >> 1);
		*mfm++ = v | mfmbits;
		lastword = v;
	}
}

static uae_u8 mfmencodetable[16] = {
	0x2a, 0x29, 0x24, 0x25, 0x12, 0x11, 0x14, 0x15,
	0x4a, 0x49, 0x44, 0x45, 0x52, 0x51, 0x54, 0x55
};


static uae_u16 dos_encode_byte (uae_u8 byte)
{
	uae_u8 b2, b1;
	uae_u16 word;

	b1 = byte;
	b2 = b1 >> 4;
	b1 &= 15;
	word = mfmencodetable[b2] <<8 | mfmencodetable[b1];
	return (word | ((word & (256 | 64)) ? 0 : 128));
}

static uae_u16 *mfmcoder (uae_u8 *src, uae_u16 *dest, int len)
{
	int i;

	for (i = 0; i < len; i++) {
		*dest = dos_encode_byte (*src++);
		*dest |= ((dest[-1] & 1)||(*dest & 0x4000)) ? 0: 0x8000;
		dest++;
	}
	return dest;
}

static void decode_pcdos (drive *drv)
{
	int i, len;
	int tr = drv->cyl * 2 + side;
	uae_u16 *dstmfmbuf, *mfm2;
	uae_u8 secbuf[1000];
	uae_u16 crc16;
	trackid *ti = drv->trackdata + tr;
	int tracklen = 12500;

	mfm2 = drv->bigmfmbuf;
	*mfm2++ = 0x9254;
	memset (secbuf, 0x4e, 40);
	memset (secbuf + 40, 0x00, 12);
	secbuf[52] = 0xc2;
	secbuf[53] = 0xc2;
	secbuf[54] = 0xc2;
	secbuf[55] = 0xfc;
	memset (secbuf + 56, 0x4e, 40);
	dstmfmbuf = mfmcoder (secbuf, mfm2, 96);
	mfm2[52] = 0x5224;
	mfm2[53] = 0x5224;
	mfm2[54] = 0x5224;
	for (i = 0; i < drv->num_secs; i++) {
		mfm2 = dstmfmbuf;
		memset (secbuf, 0x00, 12);
		secbuf[12] = 0xa1;
		secbuf[13] = 0xa1;
		secbuf[14] = 0xa1;
		secbuf[15] = 0xfe;
		secbuf[16] = drv->cyl;
		secbuf[17] = side;
		secbuf[18] = 1 + i;
		secbuf[19] = 2; // 128 << 2 = 512
		crc16 = get_crc16(secbuf + 12, 3 + 1 + 4);
		secbuf[20] = crc16 >> 8;
		secbuf[21] = crc16 & 0xff;
		memset(secbuf + 22, 0x4e, 22);
		memset(secbuf + 44, 0x00, 12);
		secbuf[56] = 0xa1;
		secbuf[57] = 0xa1;
		secbuf[58] = 0xa1;
		secbuf[59] = 0xfb;
		read_floppy_data (drv->diskfile, ti, i * 512, &secbuf[60], 512);
		crc16 = get_crc16 (secbuf + 56, 3 + 1 + 512);
		secbuf[60 + 512] = crc16 >> 8;
		secbuf[61 + 512] = crc16 & 0xff;
		len = (tracklen / 2 - 96) / drv->num_secs - 574 / drv->ddhd;
		if (len > 0)
			memset(secbuf + 512 + 62, 0x4e, len);
		dstmfmbuf = mfmcoder (secbuf, mfm2, 60 + 512 + 2 + 76 / drv->ddhd);
		mfm2[12] = 0x4489;
		mfm2[13] = 0x4489;
		mfm2[14] = 0x4489;
		mfm2[56] = 0x4489;
		mfm2[57] = 0x4489;
		mfm2[58] = 0x4489;
	}
	while (dstmfmbuf - drv->bigmfmbuf < tracklen / 2)
		*dstmfmbuf++ = 0x9254;
	drv->skipoffset = 0;
	drv->tracklen = (dstmfmbuf - drv->bigmfmbuf) * 16;
	if (disk_debug_logging > 0)
		write_log (L"pcdos read track %d\n", tr);
}

static void decode_amigados (drive *drv)
{
	/* Normal AmigaDOS format track */
	int tr = drv->cyl * 2 + side;
	int sec;
	int dstmfmoffset = 0;
	uae_u16 *dstmfmbuf = drv->bigmfmbuf;
	int len = drv->num_secs * 544 + FLOPPY_GAP_LEN;

	trackid *ti = drv->trackdata + tr;
	memset (dstmfmbuf, 0xaa, len * 2);
	dstmfmoffset += FLOPPY_GAP_LEN;
	drv->skipoffset = (FLOPPY_GAP_LEN * 8) / 3 * 2;
	drv->tracklen = len * 2 * 8;

	for (sec = 0; sec < drv->num_secs; sec++) {
		uae_u8 secbuf[544];
		uae_u16 mfmbuf[544];
		int i;
		uae_u32 deven, dodd;
		uae_u32 hck = 0, dck = 0;

		secbuf[0] = secbuf[1] = 0x00;
		secbuf[2] = secbuf[3] = 0xa1;
		secbuf[4] = 0xff;
		secbuf[5] = tr;
		secbuf[6] = sec;
		secbuf[7] = drv->num_secs - sec;

		for (i = 8; i < 24; i++)
			secbuf[i] = 0;

		read_floppy_data (drv->diskfile, ti, sec * 512, &secbuf[32], 512);

		mfmbuf[0] = mfmbuf[1] = 0xaaaa;
		mfmbuf[2] = mfmbuf[3] = 0x4489;

		deven = ((secbuf[4] << 24) | (secbuf[5] << 16)
			| (secbuf[6] << 8) | (secbuf[7]));
		dodd = deven >> 1;
		deven &= 0x55555555;
		dodd &= 0x55555555;

		mfmbuf[4] = dodd >> 16;
		mfmbuf[5] = dodd;
		mfmbuf[6] = deven >> 16;
		mfmbuf[7] = deven;

		for (i = 8; i < 48; i++)
			mfmbuf[i] = 0xaaaa;
		for (i = 0; i < 512; i += 4) {
			deven = ((secbuf[i + 32] << 24) | (secbuf[i + 33] << 16)
				| (secbuf[i + 34] << 8) | (secbuf[i + 35]));
			dodd = deven >> 1;
			deven &= 0x55555555;
			dodd &= 0x55555555;
			mfmbuf[(i >> 1) + 32] = dodd >> 16;
			mfmbuf[(i >> 1) + 33] = dodd;
			mfmbuf[(i >> 1) + 256 + 32] = deven >> 16;
			mfmbuf[(i >> 1) + 256 + 33] = deven;
		}

		for (i = 4; i < 24; i += 2)
			hck ^= (mfmbuf[i] << 16) | mfmbuf[i + 1];

		deven = dodd = hck;
		dodd >>= 1;
		mfmbuf[24] = dodd >> 16;
		mfmbuf[25] = dodd;
		mfmbuf[26] = deven >> 16;
		mfmbuf[27] = deven;

		for (i = 32; i < 544; i += 2)
			dck ^= (mfmbuf[i] << 16) | mfmbuf[i + 1];

		deven = dodd = dck;
		dodd >>= 1;
		mfmbuf[28] = dodd >> 16;
		mfmbuf[29] = dodd;
		mfmbuf[30] = deven >> 16;
		mfmbuf[31] = deven;
		mfmcode (mfmbuf + 4, 544 - 4);

		for (i = 0; i < 544; i++) {
			dstmfmbuf[dstmfmoffset % len] = mfmbuf[i];
			dstmfmoffset++;
		}
	}

	if (disk_debug_logging > 0)
		write_log (L"amigados read track %d\n", tr);
}

/*
* diskspare format
*
* 0 <4489> <4489> 0 track sector crchi, crclo, data[512] (520 bytes per sector)
*
* 0xAAAA 0x4489 0x4489 0x2AAA oddhi, oddlo, evenhi, evenlo, ...
*
* NOTE: data is MFM encoded using same method as ADOS header, not like ADOS data!
*
*/
// uint16 mfmbuf
static void decode_diskspare (drive *drv)
{
	int tr = drv->cyl * 2 + side;
	int sec;
	int dstmfmoffset = 0;
	int size = 512 + 8;
	uae_u16 *dstmfmbuf = drv->bigmfmbuf;
	int len = drv->num_secs * size + FLOPPY_GAP_LEN;

	trackid *ti = drv->trackdata + tr;
	memset (dstmfmbuf, 0xaa, len * 2);
	dstmfmoffset += FLOPPY_GAP_LEN;
	drv->skipoffset = (FLOPPY_GAP_LEN * 8) / 3 * 2;
	drv->tracklen = len * 2 * 8;

	for (sec = 0; sec < drv->num_secs; sec++) {
		uae_u8 secbuf[512 + 8];
		uae_u16 mfmbuf[512 + 8];
		int i;
		uae_u32 deven, dodd;
		uae_u16 chk;

		secbuf[0] = tr;
		secbuf[1] = sec;
		secbuf[2] = 0;
		secbuf[3] = 0;

		read_floppy_data (drv->diskfile, ti, sec * 512, &secbuf[4], 512);

		mfmbuf[0] = 0xaaaa;
		mfmbuf[1] = 0x4489;
		mfmbuf[2] = 0x4489;
		mfmbuf[3] = 0x2aaa;

		for (i = 0; i < 512; i += 4) {
			deven = ((secbuf[i + 4] << 24) | (secbuf[i + 5] << 16)
				| (secbuf[i + 6] << 8) | (secbuf[i + 7]));
			dodd = deven >> 1;
			deven &= 0x55555555;
			dodd &= 0x55555555;
			mfmbuf[i + 8 + 0] = dodd >> 16;
			mfmbuf[i + 8 + 1] = dodd;
			mfmbuf[i + 8 + 2] = deven >> 16;
			mfmbuf[i + 8 + 3] = deven;
		}
		mfmcode (mfmbuf + 8, 512);

		i = 8;
		chk = mfmbuf[i++] & 0x7fff;
		while (i < 512 + 8)
			chk ^= mfmbuf[i++];
		secbuf[2] = chk >> 8;
		secbuf[3] = chk;

		deven = ((secbuf[0] << 24) | (secbuf[1] << 16)
			| (secbuf[2] << 8) | (secbuf[3]));
		dodd = deven >> 1;
		deven &= 0x55555555;
		dodd &= 0x55555555;

		mfmbuf[4] = dodd >> 16;
		mfmbuf[5] = dodd;
		mfmbuf[6] = deven >> 16;
		mfmbuf[7] = deven;
		mfmcode (mfmbuf + 4, 4);

		for (i = 0; i < 512 + 8; i++) {
			dstmfmbuf[dstmfmoffset % len] = mfmbuf[i];
			dstmfmoffset++;
		}
	}

	if (disk_debug_logging > 0)
		write_log (L"diskspare read track %d\n", tr);
}

static void drive_fill_bigbuf (drive * drv, int force)
{
	int tr = drv->cyl * 2 + side;
	trackid *ti = drv->trackdata + tr;

	if ((!drv->diskfile && !drv->catweasel) || tr >= drv->num_tracks) {
		track_reset (drv);
		return;
	}

	if (!force && drv->catweasel) {
		drv->buffered_cyl = -1;
		return;
	}

	if (!force && drv->buffered_cyl == drv->cyl && drv->buffered_side == side)
		return;
	drv->indexoffset = 0;
	drv->multi_revolution = 0;
	drv->tracktiming[0] = 0;
	drv->skipoffset = -1;
	drv->revolutions = 1;

	if (drv->writediskfile && drv->writetrackdata[tr].bitlen > 0) {
		int i;
		trackid *wti = &drv->writetrackdata[tr];
		drv->tracklen = wti->bitlen;
		drv->revolutions = wti->revolutions;
		read_floppy_data (drv->writediskfile, wti, 0, (uae_u8*)drv->bigmfmbuf, (wti->bitlen + 7) / 8);
		for (i = 0; i < (drv->tracklen + 15) / 16; i++) {
			uae_u16 *mfm = drv->bigmfmbuf + i;
			uae_u8 *data = (uae_u8 *) mfm;
			*mfm = 256 * *data + *(data + 1);
		}
		if (disk_debug_logging > 0)
			write_log (L"track %d, length %d read from \"saveimage\"\n", tr, drv->tracklen);
	} else if (drv->filetype == ADF_CATWEASEL) {
#ifdef CATWEASEL
		drv->tracklen = 0;
		if (!catweasel_disk_changed (drv->catweasel)) {
			drv->tracklen = catweasel_fillmfm (drv->catweasel, drv->bigmfmbuf, side, drv->ddhd, 0);
		}
		drv->buffered_cyl = -1;
		if (!drv->tracklen) {
			track_reset (drv);
			return;
		}
#endif
	} else if (drv->filetype == ADF_IPF) {

#ifdef CAPS
		caps_loadtrack (drv->bigmfmbuf, drv->tracktiming, drv - floppy, tr, &drv->tracklen, &drv->multi_revolution, &drv->skipoffset);
#endif

	} else if (drv->filetype == ADF_FDI) {

#ifdef FDI2RAW
		fdi2raw_loadtrack (drv->fdi, drv->bigmfmbuf, drv->tracktiming, tr, &drv->tracklen, &drv->indexoffset, &drv->multi_revolution, 1);
#endif

	} else if (ti->type == TRACK_PCDOS) {

		decode_pcdos (drv);

	} else if (ti->type == TRACK_AMIGADOS) {

		decode_amigados (drv);

	} else if (ti->type == TRACK_DISKSPARE) {

		decode_diskspare (drv);

	} else if (ti->type == TRACK_NONE) {

		;

	} else {
		int i;
		int base_offset = ti->type == TRACK_RAW ? 0 : 1;
		drv->tracklen = ti->bitlen + 16 * base_offset;
		drv->bigmfmbuf[0] = ti->sync;
		read_floppy_data (drv->diskfile, ti, 0, (uae_u8*)(drv->bigmfmbuf + base_offset), (ti->bitlen + 7) / 8);
		for (i = base_offset; i < (drv->tracklen + 15) / 16; i++) {
			uae_u16 *mfm = drv->bigmfmbuf + i;
			uae_u8 *data = (uae_u8 *) mfm;
			*mfm = 256 * *data + *(data + 1);
		}
		if (disk_debug_logging > 1)
			write_log (L"rawtrack %d image offset=%x\n", tr, ti->offs);
	}
	drv->buffered_side = side;
	drv->buffered_cyl = drv->cyl;
	if (drv->tracklen == 0) {
		drv->tracklen = FLOPPY_WRITE_LEN * drv->ddhd * 2 * 8;
		memset (drv->bigmfmbuf, 0, FLOPPY_WRITE_LEN * 2 * drv->ddhd);
	}

	drv->trackspeed = get_floppy_speed2 (drv);
	updatemfmpos (drv);
}

/* Update ADF_EXT2 track header */
static void diskfile_update (struct zfile *diskfile, trackid *ti, int len, image_tracktype type)
{
	uae_u8 buf[2 + 2 + 4 + 4], *zerobuf;

	ti->revolutions = 1;
	ti->bitlen = len;
	zfile_fseek (diskfile, 8 + 4 + (2 + 2 + 4 + 4) * ti->track, SEEK_SET);
	memset (buf, 0, sizeof buf);
	ti->type = type;
	buf[2] = 0;
	buf[3] = ti->type;
	do_put_mem_long ((uae_u32 *) (buf + 4), ti->len);
	do_put_mem_long ((uae_u32 *) (buf + 8), ti->bitlen);
	zfile_fwrite (buf, sizeof (buf), 1, diskfile);
	if (ti->len > (len + 7) / 8) {
		zerobuf = xmalloc (uae_u8, ti->len);
		memset (zerobuf, 0, ti->len);
		zfile_fseek (diskfile, ti->offs, SEEK_SET);
		zfile_fwrite (zerobuf, 1, ti->len, diskfile);
		free (zerobuf);
	}
	if (disk_debug_logging > 0)
		write_log (L"track %d, raw track length %d written (total size %d)\n", ti->track, (ti->bitlen + 7) / 8, ti->len);
}

#define MFMMASK 0x55555555
static uae_u16 getmfmword (uae_u16 *mbuf, int shift)
{
	return (mbuf[0] << shift) | (mbuf[1] >> (16 - shift));
}

static uae_u32 getmfmlong (uae_u16 *mbuf, int shift)
{
	return ((getmfmword (mbuf, shift) << 16) | getmfmword (mbuf + 1, shift)) & MFMMASK;
}

static int decode_buffer (uae_u16 *mbuf, int cyl, int drvsec, int ddhd, int filetype, int *drvsecp, int *sectable, int checkmode)
{
	int i, secwritten = 0;
	int fwlen = FLOPPY_WRITE_LEN * ddhd;
	int length = 2 * fwlen;
	uae_u32 odd, even, chksum, id, dlong;
	uae_u8 *secdata;
	uae_u8 secbuf[544];
	uae_u16 *mend = mbuf + length;
	int shift = 0;

	memset (sectable, 0, MAX_SECTORS * sizeof (int));
	memcpy (mbuf + fwlen, mbuf, fwlen * sizeof (uae_u16));
	mend -= (4 + 16 + 8 + 512);
	while (secwritten < drvsec) {
		int trackoffs;

		while (getmfmword (mbuf, shift) != 0x4489) {
			if (mbuf >= mend)
				return 1;
			shift++;
			if (shift == 16) {
				shift = 0;
				mbuf++;
			}
		}
		while (getmfmword (mbuf, shift) == 0x4489) {
			if (mbuf >= mend)
				return 1;
			mbuf++;
		}

		odd = getmfmlong (mbuf, shift);
		even = getmfmlong (mbuf + 2, shift);
		mbuf += 4;
		id = (odd << 1) | even;

		trackoffs = (id & 0xff00) >> 8;
		if (trackoffs + 1 > drvsec) {
			write_log (L"Disk decode: weird sector number %d (%04x)\n", trackoffs, id);
			if (filetype == ADF_EXT2)
				return 2;
			continue;
		}
		chksum = odd ^ even;
		for (i = 0; i < 4; i++) {
			odd = getmfmlong (mbuf, shift);
			even = getmfmlong (mbuf + 8, shift);
			mbuf += 2;

			dlong = (odd << 1) | even;
			if (dlong && !checkmode) {
				if (filetype == ADF_EXT2)
					return 6;
				secwritten = -200;
			}
			chksum ^= odd ^ even;
		}   /* could check here if the label is nonstandard */
		mbuf += 8;
		odd = getmfmlong (mbuf, shift);
		even = getmfmlong (mbuf + 2, shift);
		mbuf += 4;
		if (((odd << 1) | even) != chksum || ((id & 0x00ff0000) >> 16) != cyl * 2 + side) {
			write_log (L"Disk decode: checksum error on sector %d header\n", trackoffs);
			if (filetype == ADF_EXT2)
				return 3;
			continue;
		}
		odd = getmfmlong (mbuf, shift);
		even = getmfmlong (mbuf + 2, shift);
		mbuf += 4;
		chksum = (odd << 1) | even;
		secdata = secbuf + 32;
		for (i = 0; i < 128; i++) {
			odd = getmfmlong (mbuf, shift);
			even = getmfmlong (mbuf + 256, shift);
			mbuf += 2;
			dlong = (odd << 1) | even;
			*secdata++ = dlong >> 24;
			*secdata++ = dlong >> 16;
			*secdata++ = dlong >> 8;
			*secdata++ = dlong;
			chksum ^= odd ^ even;
		}
		if (chksum) {
			write_log (L"Disk decode: sector %d, data checksum error\n", trackoffs);
			if (filetype == ADF_EXT2)
				return 4;
			continue;
		}
		mbuf += 256;
		sectable[trackoffs] = 1;
		secwritten++;
		memcpy (writebuffer + trackoffs * 512, secbuf + 32, 512);
	}
	if (filetype == ADF_EXT2 && (secwritten == 0 || secwritten < 0))
		return 5;
	if (secwritten == 0)
		write_log (L"Disk decode: unsupported format\n");
	if (secwritten < 0)
		write_log (L"Disk decode: sector labels ignored\n");
	*drvsecp = drvsec;
	return 0;
}

static uae_u8 mfmdecode (uae_u16 **mfmp, int shift)
{
	uae_u16 mfm = getmfmword (*mfmp, shift);
	uae_u8 out = 0;
	int i;

	(*mfmp)++;
	mfm &= 0x5555;
	for (i = 0; i < 8; i++) {
		out >>= 1;
		if (mfm & 1)
			out |= 0x80;
		mfm >>= 2;
	}
	return out;
}

static int drive_write_pcdos (drive *drv)
{
	int i;
	int drvsec = drv->num_secs;
	int fwlen = FLOPPY_WRITE_LEN * drv->ddhd;
	int length = 2 * fwlen;
	uae_u16 *mbuf = drv->bigmfmbuf;
	uae_u16 *mend = mbuf + length;
	int secwritten = 0, shift = 0, sector = -1;
	int sectable[18];
	uae_u8 secbuf[3 + 1 + 512];
	uae_u8 mark;
	uae_u16 crc;

	memset (sectable, 0, sizeof (sectable));
	memcpy (mbuf + fwlen, mbuf, fwlen * sizeof (uae_u16));
	mend -= 518;
	secbuf[0] = secbuf[1] = secbuf[2] = 0xa1;
	secbuf[3] = 0xfb;

	while (secwritten < drvsec) {
		while (getmfmword (mbuf, shift) != 0x4489) {
			if (mbuf >= mend)
				return 1;
			shift++;
			if (shift == 16) {
				shift = 0;
				mbuf++;
			}
		}
		while (getmfmword (mbuf, shift) == 0x4489) {
			if (mbuf >= mend)
				return 1;
			mbuf++;
		}
		mark = mfmdecode(&mbuf, shift);
		if (mark == 0xfe) {
			uae_u8 tmp[8];
			uae_u8 cyl, head, size;

			cyl = mfmdecode (&mbuf, shift);
			head = mfmdecode (&mbuf, shift);
			sector = mfmdecode (&mbuf, shift);
			size = mfmdecode (&mbuf, shift);
			crc = (mfmdecode (&mbuf, shift) << 8) | mfmdecode (&mbuf, shift);

			tmp[0] = 0xa1; tmp[1] = 0xa1; tmp[2] = 0xa1; tmp[3] = mark;
			tmp[4] = cyl; tmp[5] = head; tmp[6] = sector; tmp[7] = size;
			if (get_crc16 (tmp, 8) != crc || cyl != drv->cyl || head != side || size != 2 || sector < 1 || sector > drv->num_secs) {
				write_log (L"PCDOS: track %d, corrupted sector header\n", drv->cyl * 2 + side);
				return 1;
			}
			sector--;
			continue;
		}
		if (mark != 0xfb) {
			write_log (L"PCDOS: track %d: unknown address mark %02X\n", drv->cyl * 2 + side, mark);
			continue;
		}
		if (sector < 0)
			continue;
		for (i = 0; i < 512; i++)
			secbuf[i + 4] = mfmdecode (&mbuf, shift);
		crc = (mfmdecode (&mbuf, shift) << 8) | mfmdecode (&mbuf, shift);
		if (get_crc16 (secbuf, 3 + 1 + 512) != crc) {
			write_log (L"PCDOS: track %d, sector %d data checksum error\n",
				drv->cyl * 2 + side, sector + 1);
			continue;
		}
		sectable[sector] = 1;
		secwritten++;
		zfile_fseek (drv->diskfile, drv->trackdata[drv->cyl * 2 + side].offs + sector * 512, SEEK_SET);
		zfile_fwrite (secbuf + 4, sizeof (uae_u8), 512, drv->diskfile);
		write_log (L"PCDOS: track %d sector %d written\n", drv->cyl * 2 + side, sector + 1);
		sector = -1;
	}
	if (secwritten != drv->num_secs)
		write_log (L"PCDOS: track %d, %d corrupted sectors ignored\n",
		drv->cyl * 2 + side, drv->num_secs - secwritten);
	return 0;
}

static int drive_write_adf_amigados (drive *drv)
{
	int drvsec, i;
	int sectable[MAX_SECTORS];

	if (decode_buffer (drv->bigmfmbuf, drv->cyl, drv->num_secs, drv->ddhd, drv->filetype, &drvsec, sectable, 0))
		return 2;
	if (!drvsec)
		return 2;

	if (drv->filetype == ADF_EXT2)
		diskfile_update (drv->diskfile, &drv->trackdata[drv->cyl * 2 + side], drvsec * 512 * 8, TRACK_AMIGADOS);
	for (i = 0; i < drvsec; i++) {
		zfile_fseek (drv->diskfile, drv->trackdata[drv->cyl * 2 + side].offs + i * 512, SEEK_SET);
		zfile_fwrite (writebuffer + i * 512, sizeof (uae_u8), 512, drv->diskfile);
	}

	return 0;
}

/* write raw track to disk file */
static int drive_write_ext2 (uae_u16 *bigmfmbuf, struct zfile *diskfile, trackid *ti, int tracklen)
{
	int len, i;

	len = (tracklen + 7) / 8;
	if (len > ti->len) {
		write_log (L"disk raw write: image file's track %d is too small (%d < %d)!\n", ti->track, ti->len, len);
		len = ti->len;
	}
	diskfile_update (diskfile, ti, tracklen, TRACK_RAW);
	for (i = 0; i < ti->len / 2; i++) {
		uae_u16 *mfm = bigmfmbuf + i;
		uae_u16 *mfmw = bigmfmbufw + i;
		uae_u8 *data = (uae_u8 *) mfm;
		*mfmw = 256 * *data + *(data + 1);
	}
	zfile_fseek (diskfile, ti->offs, SEEK_SET);
	zfile_fwrite (bigmfmbufw, 1, len, diskfile);
	return 1;
}

static void drive_write_data (drive * drv)
{
	int ret = -1;
	int tr = drv->cyl * 2 + side;
	static int warned;

	if (drive_writeprotected (drv) || drv->trackdata[tr].type == TRACK_NONE) {
		/* read original track back because we didn't really write anything */
		drv->buffered_side = 2;
		return;
	}
	if (drv->writediskfile) {
		drive_write_ext2 (drv->bigmfmbuf, drv->writediskfile, &drv->writetrackdata[tr],
			longwritemode ? dsklength2 * 8 : drv->tracklen);
	}
	switch (drv->filetype) {
	case ADF_NORMAL:
		if (drive_write_adf_amigados (drv)) {
			if (!warned)
				notify_user (NUMSG_NEEDEXT2);
			warned = 1;
		}
		return;
	case ADF_EXT1:
		break;
	case ADF_EXT2:
		if (!longwritemode)
			ret = drive_write_adf_amigados (drv);
		if (ret) {
			write_log (L"not an amigados track %d (error %d), writing as raw track\n", drv->cyl * 2 + side, ret);
			drive_write_ext2 (drv->bigmfmbuf, drv->diskfile, &drv->trackdata[drv->cyl * 2 + side],
				longwritemode ? dsklength2 * 8 : drv->tracklen);
		}
		return;
	case ADF_IPF:
		break;
	case ADF_PCDOS:
		ret = drive_write_pcdos (drv);
		if (ret)
			write_log (L"not a PC formatted track %d (error %d)\n", drv->cyl * 2 + side, ret);
		break;
	}
	drv->tracktiming[0] = 0;
}

static void drive_eject (drive * drv)
{
#ifdef DRIVESOUND
	driveclick_insert (drv - floppy, 1);
#endif
	gui_disk_image_change (drv - floppy, NULL);
	drive_image_free (drv);
	drv->dskchange = 1;
	drv->ddhd = 1;
	drv->dskchange_time = 0;
	drv->dskready = 0;
	drv->dskready_time = 0;
	drv->dskready_down_time = 0;
	drv->crc32 = 0;
	drive_settype_id (drv); /* Back to 35 DD */
	if (disk_debug_logging > 0)
		write_log (L"eject drive %d\n", drv - &floppy[0]);
	if (input_recording > 0) {
		inprec_rstart (INPREC_DISKREMOVE);
		inprec_ru8 (drv - floppy);
		inprec_rend ();
	}
}

/* We use this function if we have no Kickstart ROM.
* No error checking - we trust our luck. */
void DISK_ersatz_read (int tr, int sec, uaecptr dest)
{
	uae_u8 *dptr = get_real_address (dest);
	zfile_fseek (floppy[0].diskfile, floppy[0].trackdata[tr].offs + sec * 512, SEEK_SET);
	zfile_fread (dptr, 1, 512, floppy[0].diskfile);
}

/* type: 0=regular, 1=ext2adf */
/* adftype: 0=DD,1=HD,2=DD PC,3=HD PC,4=525SD */
void disk_creatediskfile (TCHAR *name, int type, drive_type adftype, TCHAR *disk_name)
{
	struct zfile *f;
	int i, l, file_size, tracks, track_len, sectors;
	uae_u8 *chunk = NULL;
	uae_u8 tmp[3*4];
	uae_char *s;

	if (disk_name == NULL || _tcslen (disk_name) == 0)
		disk_name = L"empty";

	if (type == 1)
		tracks = 2 * 83;
	else
		tracks = 2 * 80;
	file_size = 880 * 1024;
	sectors = 11;
	if (adftype == 2 || adftype == 3) {
		file_size = 720 * 1024;
		sectors = 9;
	}
	track_len = FLOPPY_WRITE_LEN * 2;
	if (adftype == 1 || adftype == 3) {
		file_size *= 2;
		track_len *= 2;
	} else if (adftype == 4) {
		file_size /= 2;
		tracks /= 2;
	}

	f = zfile_fopen (name, L"wb", 0);
	chunk = xmalloc (uae_u8, 32768);
	if (f && chunk) {
		int cylsize = sectors * 2 * 512;
		memset (chunk, 0, 32768);
		if (type == 0) {
			for (i = 0; i < file_size; i += cylsize) {
				memset(chunk, 0, cylsize);
				if (adftype <= 1) {
					if (i == 0) {
						/* boot block */
						strcpy ((char*)chunk, "DOS");
					} else if (i == file_size / 2) {
						int block = file_size / 1024;
						/* root block */
						chunk[0+3] = 2;
						chunk[12+3] = 0x48;
						chunk[312] = chunk[313] = chunk[314] = chunk[315] = (uae_u8)0xff;
						chunk[316+2] = (block + 1) >> 8; chunk[316+3] = (block + 1) & 255;
						s = ua (disk_name);
						chunk[432] = strlen (s);
						strcpy ((char*)chunk + 433, s);
						xfree (s);
						chunk[508 + 3] = 1;
						disk_date (chunk + 420);
						memcpy (chunk + 472, chunk + 420, 3 * 4);
						memcpy (chunk + 484, chunk + 420, 3 * 4);
						disk_checksum(chunk, chunk + 20);
						/* bitmap block */
						memset (chunk + 512 + 4, 0xff, 2 * file_size / (1024 * 8));
						if (adftype == 0)
							chunk[512 + 0x72] = 0x3f;
						else
							chunk[512 + 0xdc] = 0x3f;
						disk_checksum(chunk + 512, chunk + 512);
					}
				}
				zfile_fwrite (chunk, cylsize, 1, f);
			}
		} else {
			l = track_len;
			zfile_fwrite ("UAE-1ADF", 8, 1, f);
			tmp[0] = 0; tmp[1] = 0; /* flags (reserved) */
			tmp[2] = 0; tmp[3] = tracks; /* number of tracks */
			zfile_fwrite (tmp, 4, 1, f);
			tmp[0] = 0; tmp[1] = 0; /* flags (reserved) */
			tmp[2] = 0; tmp[3] = 1; /* track type */
			tmp[4] = 0; tmp[5] = 0; tmp[6]=(uae_u8)(l >> 8); tmp[7] = (uae_u8)l;
			tmp[8] = 0; tmp[9] = 0; tmp[10] = 0; tmp[11] = 0;
			for (i = 0; i < tracks; i++)
				zfile_fwrite (tmp, sizeof (tmp), 1, f);
			for (i = 0; i < tracks; i++)
				zfile_fwrite (chunk, l, 1, f);
		}
	}
	xfree (chunk);
	zfile_fclose (f);
	if (f)
		DISK_history_add (name, -1, HISTORY_FLOPPY, TRUE);

}

int disk_getwriteprotect (const TCHAR *name)
{
	int needwritefile;
	drive_type drvtype;
	return diskfile_iswriteprotect (name, &needwritefile, &drvtype);
}

static void diskfile_readonly (const TCHAR *name, int readonly)
{
	struct _stat64 st;
	int mode, oldmode;

	if (stat (name, &st))
		return;
	oldmode = mode = st.st_mode;
	mode &= ~FILEFLAG_WRITE;
	if (!readonly) mode |= FILEFLAG_WRITE;
	if (mode != oldmode)
		chmod (name, mode);
}

static void setdskchangetime (drive *drv, int dsktime)
{
	int i;
	/* prevent multiple disk insertions at the same time */
	if (drv->dskchange_time > 0)
		return;
	for (i = 0; i < MAX_FLOPPY_DRIVES; i++) {
		if (&floppy[i] != drv && floppy[i].dskchange_time > 0 && floppy[i].dskchange_time + 5 >= dsktime) {
			dsktime = floppy[i].dskchange_time + 5;
		}
	}
	drv->dskchange_time = dsktime;
	if (disk_debug_logging > 0)
		write_log (L"delayed insert enable %d\n", dsktime);
}

void DISK_reinsert (int num)
{
	drive_eject (&floppy[num]);
	setdskchangetime (&floppy[num], 20);
}

int disk_setwriteprotect (int num, const TCHAR *name, int protect)
{
	int needwritefile, oldprotect;
	struct zfile *zf1, *zf2;
	int wrprot1, wrprot2, i;
	TCHAR *name2;
	drive_type drvtype;

	oldprotect = diskfile_iswriteprotect (name, &needwritefile, &drvtype);
	DISK_validate_filename (name, 1, &wrprot1, NULL, &zf1);
	if (!zf1)
		return 0;
	if (zfile_iscompressed (zf1))
		wrprot1 = 1;
	zfile_fclose (zf1);
	zf2 = getwritefile (name, &wrprot2);
	name2 = DISK_get_saveimagepath (name);

	if (needwritefile && zf2 == 0)
		disk_creatediskfile (name2, 1, drvtype, NULL);
	zfile_fclose (zf2);
	if (protect && iswritefileempty (name)) {
		for (i = 0; i < MAX_FLOPPY_DRIVES; i++) {
			if (!_tcscmp (name, floppy[i].newname))
				drive_eject (&floppy[i]);
		}
		_wunlink (name2);
	}

	if (!needwritefile)
		diskfile_readonly (name, protect);
	diskfile_readonly (name2, protect);
	DISK_reinsert (num);
	return 1;
}

void disk_eject (int num)
{
	config_changed = 1;
	gui_filename (num, L"");
	drive_eject (floppy + num);
	*currprefs.df[num] = *changed_prefs.df[num] = 0;
	floppy[num].newname[0] = 0;
	update_drive_gui (num);
}

int DISK_history_add (const TCHAR *name, int idx, int type, int donotcheck)
{
	int i;

	if (idx >= MAX_PREVIOUS_FLOPPIES)
		return 0;
	if (name == NULL) {
		dfxhistory[type][idx][0] = 0;
		return 1;
	}
	if (name[0] == 0)
		return 0;
	if (!donotcheck) {
		if (!zfile_exists (name))
			return 0;
	}
	if (idx >= 0) {
		if (idx >= MAX_PREVIOUS_FLOPPIES)
			return 0;
		dfxhistory[type][idx][0] = 0;
		for (i = 0; i < MAX_PREVIOUS_FLOPPIES; i++) {
			if (!_tcscmp (dfxhistory[type][i], name))
				return 0;
		}
		_tcscpy (dfxhistory[type][idx], name);
		return 1;
	}
	for (i = 0; i < MAX_PREVIOUS_FLOPPIES; i++) {
		if (!_tcscmp (dfxhistory[type][i], name)) {
			while (i < MAX_PREVIOUS_FLOPPIES - 1) {
				_tcscpy (dfxhistory[type][i], dfxhistory[type][i + 1]);
				i++;
			}
			dfxhistory[type][MAX_PREVIOUS_FLOPPIES - 1][0] = 0;
			break;
		}
	}
	for (i = MAX_PREVIOUS_FLOPPIES - 2; i >= 0; i--)
		_tcscpy (dfxhistory[type][i + 1], dfxhistory[type][i]);
	_tcscpy (dfxhistory[type][0], name);
	return 1;
}

TCHAR *DISK_history_get (int idx, int type)
{
	if (idx >= MAX_PREVIOUS_FLOPPIES)
		return NULL;
	return dfxhistory[type][idx];
}

static void disk_insert_2 (int num, const TCHAR *name, int forced)
{
	drive *drv = floppy + num;

	if (forced) {
		drive_insert (drv, &currprefs, num, name);
		return;
	}
	if (!_tcscmp (currprefs.df[num], name))
		return;
	_tcscpy (drv->newname, name);
	_tcscpy (currprefs.df[num], name);
	DISK_history_add (name, -1, HISTORY_FLOPPY, 0);
	if (name[0] == 0) {
		disk_eject (num);
	} else if (!drive_empty(drv) || drv->dskchange_time > 0) {
		drive_eject (drv);
		/* set dskchange_time, disk_insert() will be
		* called from DISK_check_change() after 2 second delay
		* this makes sure that all programs detect disk change correctly
		*/
		setdskchangetime (drv, 20);
	} else {
		setdskchangetime (drv, 1);
	}
}

void disk_insert (int num, const TCHAR *name)
{
	config_changed = 1;
	target_addtorecent (name, 0);
	disk_insert_2 (num, name, 0);
}
void disk_insert_force (int num, const TCHAR *name)
{
	disk_insert_2 (num, name, 1);
}

void DISK_check_change (void)
{
	int i;

	if (currprefs.floppy_speed != changed_prefs.floppy_speed)
		currprefs.floppy_speed = changed_prefs.floppy_speed;
	for (i = 0; i < MAX_FLOPPY_DRIVES; i++) {
		drive *drv = floppy + i;
		gui_lock ();
		if (currprefs.dfxtype[i] != changed_prefs.dfxtype[i]) {
			currprefs.dfxtype[i] = changed_prefs.dfxtype[i];
			reset_drive (i);
#ifdef RETROPLATFORM
			rp_floppydrive_change (i, currprefs.dfxtype[i] >= 0 ? 1 : 0);
#endif
		}
		if (drv->dskchange_time == 0 && _tcscmp (currprefs.df[i], changed_prefs.df[i]))
			disk_insert (i, changed_prefs.df[i]);
		gui_unlock ();
		if (drv->dskready_down_time > 0)
			drv->dskready_down_time--;
		/* emulate drive motor turn on time */
		if (drv->dskready_time > 0 && !drive_empty(drv)) {
			drv->dskready_time--;
			if (drv->dskready_time == 0)
				drv->dskready = 1;
		}
		/* delay until new disk image is inserted */
		if (drv->dskchange_time) {
			drv->dskchange_time--;
			if (drv->dskchange_time == 0) {
				drive_insert (drv, &currprefs, i, drv->newname);
				if (disk_debug_logging > 0)
					write_log (L"delayed insert, drive %d, image '%s'\n", i, drv->newname);
				update_drive_gui (i);

			}
		}
	}
}

int disk_empty (int num)
{
	return drive_empty (floppy + num);
}

static TCHAR *tobin(uae_u8 v)
{
	int i;
	static TCHAR buf[10];
	for( i = 7; i >= 0; i--)
		buf[7 - i] = v & (1 << i) ? '1' : '0';
	buf[i] = 0;
	return buf;
}

void DISK_select (uae_u8 data)
{
	int step_pulse, lastselected, dr;
	static uae_u8 prevdata;
	static int step;

	lastselected = selected;
	selected = (data >> 3) & 15;
	side = 1 - ((data >> 2) & 1);
	direction = (data >> 1) & 1;
	step_pulse = data & 1;

	if (disk_debug_logging > 1)
		write_log (L"%08X %02X %s drvmask=%x", M68K_GETPC, data, tobin(data), selected ^ 15);

#ifdef AMAX
	if (currprefs.amaxromfile[0])
		amax_disk_select (data, prevdata);
#endif

	if ((prevdata & 0x80) != (data & 0x80)) {
		for (dr = 0; dr < 4; dr++) {
			if (floppy[dr].indexhackmode > 1 && !(selected & (1 << dr))) {
				floppy[dr].indexhack = 1;
				if (disk_debug_logging > 1)
					write_log (L" indexhack!");
			}
		}
	}

	if (disk_debug_logging > 1) {
		write_log (L" %d%d%d%d% ", (selected & 1) ? 0 : 1, (selected & 2) ? 0 : 1, (selected & 4) ? 0 : 1, (selected & 8) ? 0 : 1);
		if ((prevdata & 0x80) != (data & 0x80))
			write_log (L" dskmotor %d ", (data & 0x80) ? 1 : 0);
		if ((prevdata & 0x02) != (data & 0x02))
			write_log (L" direct %d ", (data & 0x02) ? 1 : 0);
		if ((prevdata & 0x04) != (data & 0x04))
			write_log (L" side %d ", (data & 0x04) ? 1 : 0);
	}

	if (step != step_pulse) {
		if (disk_debug_logging > 1)
			write_log (L" dskstep %d ", step_pulse);
		step = step_pulse;
		if (step && !savestate_state) {
			for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
				if (!((selected | disabled) & (1 << dr))) {
					drive_step (floppy + dr);
					if (floppy[dr].indexhackmode > 1 && (data & 0x80))
						floppy[dr].indexhack = 1;
				}
			}
		}
	}
	if (!savestate_state) {
		for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
			drive *drv = floppy + dr;
			/* motor on/off workings tested with small assembler code on real Amiga 1200. */
			/* motor/id flipflop is set only when drive select goes from high to low */
			if (!(selected & (1 << dr)) && (lastselected & (1 << dr)) ) {
				drv->drive_id_scnt++;
				drv->drive_id_scnt &= 31;
				drv->idbit = (drv->drive_id & (1L << (31 - drv->drive_id_scnt))) ? 1 : 0;
				if (!(disabled & (1 << dr))) {
					if ((prevdata & 0x80) == 0 || (data & 0x80) == 0) {
						/* motor off: if motor bit = 0 in prevdata or data -> turn motor on */
						drive_motor (drv, 0);
					} else if (prevdata & 0x80) {
						/* motor on: if motor bit = 1 in prevdata only (motor flag state in data has no effect)
						-> turn motor off */
						drive_motor (drv, 1);
					}
				}
				if (!currprefs.cs_df0idhw && dr == 0)
					drv->idbit = 0;
#ifdef DEBUG_DRIVE_ID
				write_log (L"DISK_status: sel %d id %s (%08X) [0x%08lx, bit #%02d: %d]\n",
					dr, drive_id_name(drv), drv->drive_id, drv->drive_id << drv->drive_id_scnt, 31 - drv->drive_id_scnt, drv->idbit);
#endif
			}
		}
	}

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		floppy[dr].state = (!(selected & (1 << dr))) | !floppy[dr].motoroff;
		update_drive_gui (dr);
	}
	prevdata = data;
	if (disk_debug_logging > 1)
		write_log (L"\n");
}

uae_u8 DISK_status (void)
{
	uae_u8 st = 0x3c;
	int dr;

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = floppy + dr;
		if (drv->amax) {
			st = amax_disk_status ();
		} else if (!((selected | disabled) & (1 << dr))) {
			if (drive_running (drv)) {
				if (drv->catweasel) {
#ifdef CATWEASEL
					if (catweasel_diskready (drv->catweasel))
						st &= ~0x20;
#endif
				} else {
					if (drv->dskready && !drv->indexhack && currprefs.dfxtype[dr] != DRV_35_DD_ESCOM)
						st &= ~0x20;
				}
			} else {
				if (currprefs.cs_df0idhw || dr > 0) {
					/* report drive ID */
					if (drv->idbit && currprefs.dfxtype[dr] != DRV_35_DD_ESCOM)
						st &= ~0x20;
				} else {
					/* non-ID internal drive: mirror real dskready */
					if (drv->dskready)
						st &= ~0x20;
				}
				/* dskrdy needs some cycles after switching the motor off.. (Pro Tennis Tour) */
				if (!currprefs.cs_df0idhw && dr == 0 && drv->motordelay)
					st &= ~0x20;
			}
			if (drive_track0 (drv))
				st &= ~0x10;
			if (drive_writeprotected (drv))
				st &= ~8;
			if (drv->catweasel) {
#ifdef CATWEASEL
				if (catweasel_disk_changed (drv->catweasel))
					st &= ~4;
#endif
			} else if (drv->dskchange && currprefs.dfxtype[dr] != DRV_525_SD) {
				st &= ~4;
			}
		} else if (!(selected & (1 << dr))) {
			if (drv->idbit)
				st &= ~0x20;
		}
	}
	return st;
}

static int unformatted (drive *drv)
{
	int tr = drv->cyl * 2 + side;
	if (tr >= drv->num_tracks)
		return 1;
	if (drv->filetype == ADF_EXT2 && drv->trackdata[tr].bitlen == 0)
		return 1;
	if (drv->trackdata[tr].type == TRACK_NONE)
		return 1;
	return 0;
}

/* get one bit from MFM bit stream */
STATIC_INLINE uae_u32 getonebit (uae_u16 * mfmbuf, int mfmpos)
{
	uae_u16 *buf;

	buf = &mfmbuf[mfmpos >> 4];
	return (buf[0] & (1 << (15 - (mfmpos & 15)))) ? 1 : 0;
}

void dumpdisk (void)
{
	int i, j, k;
	uae_u16 w;

	for (i = 0; i < MAX_FLOPPY_DRIVES; i++) {
		drive *drv = &floppy[i];
		if (!(disabled & (1 << i))) {
			console_out_f (L"Drive %d: motor %s cylinder %2d sel %s %s mfmpos %d/%d\n",
				i, drv->motoroff ? L"off" : L" on", drv->cyl, (selected & (1 << i)) ? L"no" : L"yes",
				drive_writeprotected(drv) ? L"ro" : L"rw", drv->mfmpos, drv->tracklen);
			w = word;
			for (j = 0; j < 15; j++) {
				console_out_f (L"%04X ", w);
				for (k = 0; k < 16; k++) {
					w <<= 1;
					w |= getonebit (drv->bigmfmbuf, drv->mfmpos + j * 16 + k);
				}
			}
			console_out (L"\n");
		}
	}
	console_out_f (L"side %d, dma %d, bitoffset %d, word %04X, dskbytr %04X adkcon %04X dsksync %04X\n", side, dskdmaen, bitoffset, word, dskbytr_val, adkcon, dsksync);
}

static void disk_dmafinished (void)
{
	INTREQ (0x8000 | 0x0002);
	longwritemode = 0;
	dskdmaen = 0;
	if (disk_debug_logging > 0) {
		int dr, mfmpos = -1;
		write_log (L"disk dma finished %08X MFMpos=", dskpt);
		for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++)
			write_log (L"%d%s", floppy[dr].mfmpos, dr < MAX_FLOPPY_DRIVES - 1 ? L"," : L"");
		write_log (L"\n");
	}
}

static void fetchnextrevolution (drive *drv)
{
	drv->trackspeed = get_floppy_speed2 (drv);
	if (!drv->multi_revolution)
		return;
	switch (drv->filetype)
	{
	case ADF_IPF:
#ifdef CAPS
		caps_loadrevolution (drv->bigmfmbuf, drv - floppy, drv->cyl * 2 + side, &drv->tracklen);
#endif
		break;
	case ADF_FDI:
#ifdef FDI2RAW
		fdi2raw_loadrevolution (drv->fdi, drv->bigmfmbuf, drv->tracktiming, drv->cyl * 2 + side, &drv->tracklen, 1);
#endif
		break;
	}
}

void DISK_handler (uae_u32 data)
{
	int flag = diskevent_flag;

	event2_remevent(ev2_disk);
	DISK_update (disk_sync_cycle);
	if (flag & (DISK_REVOLUTION << 0))
		fetchnextrevolution (&floppy[0]);
	if (flag & (DISK_REVOLUTION << 1))
		fetchnextrevolution (&floppy[1]);
	if (flag & (DISK_REVOLUTION << 2))
		fetchnextrevolution (&floppy[2]);
	if (flag & (DISK_REVOLUTION << 3))
		fetchnextrevolution (&floppy[3]);
	if (flag & DISK_WORDSYNC)
		INTREQ (0x8000 | 0x1000);
	if (flag & DISK_INDEXSYNC)
		cia_diskindex ();
#if 0
	{
		int i;
		for (i = 0; i < MAX_FLOPPY_DRIVES; i++) {
			drive *drv = &floppy[i];
			if (drv->dskready_time) {
				drv->dskready_time--;
				if (drv->dskready_time == 0) {
					drv->dskready = 1;
					if (disk_debug_logging > 0)
						write_log (L"%d: %d\n", i, drv->mfmpos);
				}
			}
		}
	}
#endif
}

#ifdef CPUEMU_12
extern uae_u8 cycle_line[256];

static void diskdma (uae_u32 pt, uae_u16 w, int write)
{
	int i, got;

	got = 0;
	for (i = 7; i <= 11; i += 2) {
		if (!cycle_line[i]) {
			cycle_line[i] = CYCLE_MISC;
			if (debug_dma)
				record_dma (write ? 0x26 : 0x08, w, pt, i, vpos, DMARECORD_DISK);
			got = 1;
			break;
		}
		//	if (cycle_line[i] != CYCLE_MISC)
		//	    write_log (L"%d!?\n", cycle_line[i]);
	}
	//    if (!got)
	//	write_log (L"disk dma cycle overflow!?\n");
}
#endif

static void disk_doupdate_write (drive * drv, int floppybits)
{
	int dr;
	int drives[4];

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv2 = &floppy[dr];
		drives[dr] = 0;
		if (drv2->motoroff)
			continue;
		if (selected & (1 << dr))
			continue;
		drives[dr] = 1;
	}

	while (floppybits >= drv->trackspeed) {
		for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
			if (drives[dr]) {
				floppy[dr].mfmpos++;
				floppy[dr].mfmpos %= drv->tracklen;
			}
		}
		if ((dmacon & 0x210) == 0x210 && dskdmaen == 3 && dsklength > 0 && (!(adkcon &0x400) || dma_enable)) {
			bitoffset++;
			bitoffset &= 15;
			if (!bitoffset) {
				for (dr = 0; dr < MAX_FLOPPY_DRIVES ; dr++) {
					drive *drv2 = &floppy[dr];
					uae_u16 w = get_word (dskpt);
#ifdef CPUEMU_12
					diskdma (dskpt, w, 1);
#endif
					if (drives[dr]) {
						drv2->bigmfmbuf[drv2->mfmpos >> 4] = w;
						drv2->bigmfmbuf[(drv2->mfmpos >> 4) + 1] = 0x5555;
						drv2->writtento = 1;
					}
#ifdef AMAX
					if (currprefs.amaxromfile[0])
						amax_diskwrite (w);
#endif
				}
				dskpt += 2;
				dsklength--;
				if (dsklength == 0) {
					disk_dmafinished ();
					for (dr = 0; dr < MAX_FLOPPY_DRIVES ; dr++) {
						drive *drv2 = &floppy[dr];
						drv2->writtento = 0;
						if (drives[dr]) {
							drive_write_data (drv2);
							//set_steplimit (drv2);
						}
					}
				}
			}
		}
		floppybits -= drv->trackspeed;
	}
}

static void updatetrackspeed (drive *drv, int mfmpos)
{
	if (dskdmaen < 3) {
		uae_u16 *p = drv->tracktiming;
		p += mfmpos / 8;
		drv->trackspeed = get_floppy_speed2 (drv);
		drv->trackspeed = drv->trackspeed * p[0] / 1000;
		if (drv->trackspeed < 700 || drv->trackspeed > 3000) {
			static int warned;
			warned++;
			if (warned < 50)
				write_log (L"corrupted trackspeed value %d\n", drv->trackspeed);
			drv->trackspeed = 1000;
		}
	}
}

static void disk_doupdate_predict (drive * drv, int startcycle)
{
	int is_sync = 0;
	int firstcycle = startcycle;
	uae_u32 tword = word;
	int mfmpos = drv->mfmpos;
	int indexhack = drv->indexhack;

	diskevent_flag = 0;
	while (startcycle < (maxhpos << 8) && !diskevent_flag) {
		int cycle = startcycle >> 8;
		if (drv->tracktiming[0])
			updatetrackspeed (drv, mfmpos);
		if (dskdmaen != 3) {
			tword <<= 1;
			if (!drive_empty (drv)) {
				if (unformatted (drv))
					tword |= (uaerand() & 0x1000) ? 1 : 0;
				else
					tword |= getonebit (drv->bigmfmbuf, mfmpos);
			}
			if ((tword & 0xffff) == dsksync && dsksync != 0)
				diskevent_flag |= DISK_WORDSYNC;
		}
		mfmpos++;
		mfmpos %= drv->tracklen;
		if (mfmpos == 0)
			diskevent_flag |= DISK_REVOLUTION << (drv - floppy);
		if (mfmpos == drv->indexoffset) {
			diskevent_flag |= DISK_INDEXSYNC;
			indexhack = 0;
		}
		if (dskdmaen != 3 && mfmpos == drv->skipoffset) {
			int skipcnt = disk_jitter;
			while (skipcnt-- > 0) {
				mfmpos++;
				mfmpos %= drv->tracklen;
				if (mfmpos == 0)
					diskevent_flag |= DISK_REVOLUTION << (drv - floppy);
				if (mfmpos == drv->indexoffset) {
					diskevent_flag |= DISK_INDEXSYNC;
					indexhack = 0;
				}
			}
		}
		startcycle += drv->trackspeed;
	}
	if (drv->tracktiming[0])
		updatetrackspeed (drv, drv->mfmpos);
	if (diskevent_flag) {
		disk_sync_cycle = startcycle >> 8;
		event2_newevent (ev2_disk, (startcycle - firstcycle) / CYCLE_UNIT);
	}
}

static void disk_doupdate_read_nothing (int floppybits)
{
	int j = 0, k = 1, l = 0;

	while (floppybits >= get_floppy_speed()) {
		word <<= 1;
		if (bitoffset == 15 && dma_enable && dskdmaen == 2 && dsklength >= 0) {
			if (dsklength > 0) {
				put_word (dskpt, word);
#ifdef CPUEMU_12
				diskdma (dskpt, word, 0);
#endif
				dskpt += 2;
			}
			dsklength--;
			if (dsklength <= 0)
				disk_dmafinished ();
		}
		if ((bitoffset & 7) == 7) {
			dskbytr_val = word & 0xff;
			dskbytr_val |= 0x8000;
		}
		bitoffset++;
		bitoffset &= 15;
		floppybits -= get_floppy_speed();
	}
}

static void disk_doupdate_read (drive * drv, int floppybits)
{
	int j = 0, k = 1, l = 0;

	/*
	uae_u16 *mfmbuf = drv->bigmfmbuf;
	dsksync = 0x4444;
	adkcon |= 0x400;
	drv->mfmpos = 0;
	memset (mfmbuf, 0, 1000);
	cycles = 0x1000000;
	// 4444 4444 4444 aaaa aaaaa 4444 4444 4444
	// 4444 aaaa aaaa 4444
	mfmbuf[0] = 0x4444;
	mfmbuf[1] = 0x4444;
	mfmbuf[2] = 0x4444;
	mfmbuf[3] = 0xaaaa;
	mfmbuf[4] = 0xaaaa;
	mfmbuf[5] = 0x4444;
	mfmbuf[6] = 0x4444;
	mfmbuf[7] = 0x4444;
	*/
	while (floppybits >= drv->trackspeed) {
		if (drv->tracktiming[0])
			updatetrackspeed (drv, drv->mfmpos);
		word <<= 1;
		if (!drive_empty (drv)) {
			if (unformatted (drv))
				word |= (uaerand() & 0x1000) ? 1 : 0;
			else
				word |= getonebit (drv->bigmfmbuf, drv->mfmpos);
		}
		//write_log (L"%08X bo=%d so=%d mfmpos=%d dma=%d\n", (word & 0xffffff), bitoffset, syncoffset, drv->mfmpos,dma_enable);
		drv->mfmpos++;
		drv->mfmpos %= drv->tracklen;
		if (drv->mfmpos == drv->indexoffset) {
			if (disk_debug_logging > 1 && drv->indexhack)
				write_log (L"indexhack cleared\n");
			drv->indexhack = 0;
		}
		if (drv->mfmpos == drv->skipoffset) {
			drv->mfmpos += disk_jitter;
			drv->mfmpos %= drv->tracklen;
		}
		if (bitoffset == 15 && dma_enable && dskdmaen == 2 && dsklength >= 0) {
			if (dsklength > 0) {
				put_word (dskpt, word);
#ifdef CPUEMU_12
				diskdma (dskpt, word, 0);
#endif
				dskpt += 2;
			}
#if 0
			dma_tab[j++] = word;
			if (j == MAX_DISK_WORDS_PER_LINE - 1) {
				write_log (L"Bug: Disk DMA buffer overflow!\n");
				j--;
			}
#endif
			dsklength--;
			if (dsklength <= 0)
				disk_dmafinished ();
		}
		if ((bitoffset & 7) == 7) {
			dskbytr_val = word & 0xff;
			dskbytr_val |= 0x8000;
		}
		if (word == dsksync) {
			if (adkcon & 0x400)
				bitoffset = 15;
			if (dskdmaen) {
				if (disk_debug_logging && dma_enable == 0)
					write_log (L"Sync match, DMA started at %d\n", drv->mfmpos);
				dma_enable = 1;
			}
		}
		bitoffset++;
		bitoffset &= 15;
		floppybits -= drv->trackspeed;
	}
#if 0
	dma_tab[j] = 0xffffffff;
#endif
}

static void disk_dma_debugmsg (void)
{
	write_log (L"LEN=%04X (%d) SYNC=%04X PT=%08X ADKCON=%04X PC=%08X\n",
		dsklength, dsklength, (adkcon & 0x400) ? dsksync : 0xffff, dskpt, adkcon, M68K_GETPC);
}

#if 0
/* disk DMA fetch happens on real Amiga at the beginning of next horizontal line
(cycles 9, 11 and 13 according to hardware manual) We transfer all DMA'd
data at cycle 0. I don't think any program cares about this small difference.
*/
static void dodmafetch (void)
{
	int i;

	i = 0;
	while (dma_tab[i] != 0xffffffff && dskdmaen != 3 && (dmacon & 0x210) == 0x210) {
		put_word (dskpt, dma_tab[i++]);
		dskpt += 2;
	}
	dma_tab[0] = 0xffffffff;
}
#endif

/* this is very unoptimized. DSKBYTR is used very rarely, so it should not matter. */

uae_u16 DSKBYTR (int hpos)
{
	uae_u16 v;

	DISK_update (hpos);
	v = dskbytr_val;
	dskbytr_val &= ~0x8000;
	if (word == dsksync)
		v |= 0x1000;
	if (dskdmaen && (dmacon & 0x210) == 0x210)
		v |= 0x4000;
	if (dsklen & 0x4000)
		v |= 0x2000;
	if (disk_debug_logging > 1)
		write_log (L"DSKBYTR=%04X hpos=%d\n", v, hpos);
	if (disk_debug_mode & DISK_DEBUG_PIO) {
		int dr;
		for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
			drive *drv = &floppy[dr];
			if (drv->motoroff)
				continue;
			if (!(selected & (1 << dr))) {
				if (disk_debug_track < 0 || disk_debug_track == 2 * drv->cyl + side) {
					disk_dma_debugmsg ();
					write_log (L"DSKBYTR=%04X\n", v);
					activate_debugger ();
					break;
				}
			}
		}
	}
	return v;
}

static void DISK_start (void)
{
	int dr;

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		if (!(selected & (1 << dr))) {
			int tr = drv->cyl * 2 + side;
			trackid *ti = drv->trackdata + tr;

			if (dskdmaen == 3) {
				drv->tracklen = longwritemode ? FLOPPY_WRITE_MAXLEN : FLOPPY_WRITE_LEN * drv->ddhd * 8 * 2;
				drv->trackspeed = get_floppy_speed ();
				drv->skipoffset = -1;
				updatemfmpos (drv);
			}
			/* Ugh.  A nasty hack.  Assume ADF_EXT1 tracks are always read
			from the start.  */
			if (ti->type == TRACK_RAW1)
				drv->mfmpos = 0;
			if (drv->catweasel)
				drive_fill_bigbuf (drv, 1);
		}
		drv->floppybitcounter = 0;
	}
}

static int linecounter;

void DISK_hsync (int tohpos)
{
	int dr;

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		if (drv->steplimit)
			drv->steplimit--;
	}
	if (linecounter) {
		linecounter--;
		if (! linecounter)
			disk_dmafinished ();
		return;
	}
	DISK_update (tohpos);
}

void DISK_update (int tohpos)
{
	int dr;
	int cycles = (tohpos << 8) - disk_hpos;
	int startcycle = disk_hpos;
	int didread;

	disk_jitter = ((uaerand () >> 4) & 3) + 1;
	if (disk_jitter > 2)
		disk_jitter = 1;
	if (cycles <= 0)
		return;
	disk_hpos += cycles;
	if (disk_hpos >= (maxhpos << 8))
		disk_hpos -= maxhpos << 8;

#if 0
	dodmafetch ();
#endif

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];

		if (drv->motoroff || !drv->tracklen || !drv->trackspeed)
			continue;
		drv->floppybitcounter += cycles;
		if (selected & (1 << dr)) {
			drv->mfmpos += drv->floppybitcounter / drv->trackspeed;
			drv->mfmpos %= drv->tracklen;
			drv->floppybitcounter %= drv->trackspeed;
			continue;
		}
		if (drv->diskfile)
			drive_fill_bigbuf (drv, 0);
		drv->mfmpos %= drv->tracklen;
	}
	didread = 0;
	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		if (drv->motoroff || !drv->trackspeed)
			continue;
		if (selected & (1 << dr))
			continue;
		if (dskdmaen == 3)
			disk_doupdate_write (drv, drv->floppybitcounter);
		else
			disk_doupdate_read (drv, drv->floppybitcounter);
		disk_doupdate_predict (drv, disk_hpos);
		drv->floppybitcounter %= drv->trackspeed;
		didread = 1;
		break;
	}
	/* no floppy selected but read dma */
	if (!didread && dskdmaen == 2) {
		disk_doupdate_read_nothing (cycles);
	}

}

void DSKLEN (uae_u16 v, int hpos)
{
	int dr, prev = dsklen;
	int noselected = 0;
	int motormask;

	DISK_update (hpos);
	if ((v & 0x8000) && (dsklen & 0x8000)) {
		dskdmaen = 2;
		DISK_start ();
		if (!(v & 0x4000))
			dma_enable = (adkcon & 0x400) ? 0 : 1;
	}
	if (!(v & 0x8000)) {
		if (dskdmaen) {
			/* Megalomania and Knightmare does this */
			if (disk_debug_logging > 0 && dskdmaen == 2)
				write_log (L"warning: Disk read DMA aborted, %d words left PC=%x\n", dsklength, M68K_GETPC);
			if (dskdmaen == 3) {
				write_log (L"warning: Disk write DMA aborted, %d words left PC=%x\n", dsklength, M68K_GETPC);
				// did program write something that needs to be stored to file?
				for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
					drive *drv2 = &floppy[dr];
					if (!drv2->writtento)
						continue;
					drive_write_data (drv2);
				}
			}
			dskdmaen = 0;
		}
	}
	dsklen = v;
	dsklength2 = dsklength = dsklen & 0x3fff;

	if (dskdmaen == 0)
		return;

	if ((v & 0x4000) && (prev & 0x4000)) {
		if (dsklength == 0)
			return;
		if (dsklength == 1) {
			disk_dmafinished ();
			return;
		}
		dskdmaen = 3;
		DISK_start ();
	}

	if (dsklength == 1)
		dsklength = 0;

	if (((disk_debug_mode & DISK_DEBUG_DMA_READ) && dskdmaen == 2) ||
		((disk_debug_mode & DISK_DEBUG_DMA_WRITE) && dskdmaen == 3))
	{
		for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
			drive *drv = &floppy[dr];
			if (drv->motoroff)
				continue;
			if (!(selected & (1 << dr))) {
				if (disk_debug_track < 0 || disk_debug_track == 2 * drv->cyl + side) {
					disk_dma_debugmsg ();
					activate_debugger ();
					break;
				}
			}
		}
	}

	motormask = 0;
	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		drv->writtento = 0;
		if (drv->motoroff)
			continue;
		motormask |= 1 << dr;
		if ((selected & (1 << dr)) == 0)
			break;
	}
	if (dr == 4) {
		write_log (L"disk %s DMA started, drvmask=%x motormask=%x\n",
			dskdmaen == 3 ? L"write" : L"read", selected ^ 15, motormask);
		noselected = 1;
	} else {
		if (disk_debug_logging > 0) {
			write_log (L"disk %s DMA started, drvmask=%x track %d mfmpos %d dmaen=%d\n",
				dskdmaen == 3 ? L"write" : L"read", selected ^ 15,
				floppy[dr].cyl * 2 + side, floppy[dr].mfmpos, dma_enable);
			disk_dma_debugmsg ();
		}
	}

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++)
		update_drive_gui (dr);

	/* Try to make floppy access from Kickstart faster.  */
	if (dskdmaen != 2 && dskdmaen != 3)
		return;

	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		if (selected & (1 << dr))
			continue;
		if (drv->filetype != ADF_NORMAL)
			break;
	}
	if (dr < MAX_FLOPPY_DRIVES) /* no turbo mode if any selected drive has non-standard ADF */
		return;
	{
		int done = 0;
		for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
			drive *drv = &floppy[dr];
			int pos, i;

			if (drv->motoroff)
				continue;
			if (!drv->useturbo && currprefs.floppy_speed > 0)
				continue;
			if (selected & (1 << dr))
				continue;

			pos = drv->mfmpos & ~15;
			drive_fill_bigbuf (drv, 0);

			if (dskdmaen == 2) { /* TURBO read */

				if (adkcon & 0x400) {
					for (i = 0; i < drv->tracklen; i += 16) {
						pos += 16;
						pos %= drv->tracklen;
						if (drv->bigmfmbuf[pos >> 4] == dsksync) {
							/* must skip first disk sync marker */
							pos += 16;
							pos %= drv->tracklen;
							break;
						}
					}
					if (i >= drv->tracklen)
						return;
				}
				while (dsklength-- > 0) {
					put_word (dskpt, drv->bigmfmbuf[pos >> 4]);
					dskpt += 2;
					pos += 16;
					pos %= drv->tracklen;
				}
				INTREQ (0x8000 | 0x1000);
				done = 1;

			} else if (dskdmaen == 3) { /* TURBO write */

				for (i = 0; i < dsklength; i++) {
					uae_u16 w = get_word (dskpt + i * 2);
					drv->bigmfmbuf[pos >> 4] = w;
#ifdef AMAX
					if (currprefs.amaxromfile[0])
						amax_diskwrite (w);
#endif
					pos += 16;
					pos %= drv->tracklen;
				}
				drive_write_data (drv);
				done = 1;
			}
		}
		if (!done && noselected) {
			while (dsklength-- > 0) {
				if (dskdmaen == 3) {
					uae_u16 w = get_word (dskpt);
#ifdef AMAX
					if (currprefs.amaxromfile[0])
						amax_diskwrite (w);
#endif
				} else {
					put_word (dskpt, 0);
				}
				dskpt += 2;
			}
			INTREQ (0x8000 | 0x1000);
			done = 1;
		}

		if (done) {
			linecounter = 2;
			dskdmaen = 0;
			return;
		}
	}
}

void DSKSYNC (int hpos, uae_u16 v)
{
	if (v == dsksync)
		return;
	DISK_update (hpos);
	dsksync = v;
}

void DSKDAT (uae_u16 v)
{
	static int count = 0;
#if 0
	if (dsklen == 0x8000) {
		if (v == 1)
			longwritemode = 1;
		return;
	}
#endif
	if (count < 5) {
		count++;
		write_log (L"%04X written to DSKDAT. Not good. PC=%08X", v, M68K_GETPC);
		if (count == 5)
			write_log (L"(further messages suppressed)");

		write_log (L"\n");
	}
}
void DSKPTH (uae_u16 v)
{
	dskpt = (dskpt & 0xffff) | ((uae_u32) v << 16);
}

void DSKPTL (uae_u16 v)
{
	dskpt = (dskpt & ~0xffff) | (v);
}

void DISK_free (void)
{
	int dr;
	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		drive_image_free (drv);
	}
}

void DISK_init (void)
{
	int dr;

#if 0
	dma_tab[0] = 0xffffffff;
#endif
	for (dr = 0; dr < MAX_FLOPPY_DRIVES; dr++) {
		drive *drv = &floppy[dr];
		/* reset all drive types to 3.5 DD */
		drive_settype_id (drv);
		if (!drive_insert (drv, &currprefs, dr, currprefs.df[dr]))
			disk_eject (dr);
	}
	if (disk_empty (0))
		write_log (L"No disk in drive 0.\n");
	amax_init ();
}

void DISK_reset (void)
{
	int i;

	if (savestate_state)
		return;

	//floppy[0].catweasel = &cwc.drives[0];
	disk_hpos = 0;
	dskdmaen = 0;
	disabled = 0;
	for (i = 0; i < MAX_FLOPPY_DRIVES; i++)
		reset_drive (i);
	setamax ();
}

int DISK_examine_image (struct uae_prefs *p, int num, uae_u32 *crc32)
{
	int drvsec;
	int ret, i;
	drive *drv = &floppy[num];
	uae_u32 dos, crc, crc2;
	int wasdelayed = drv->dskchange_time;
	int sectable[MAX_SECTORS];

	ret = 0;
	drv->cyl = 0;
	side = 0;
	*crc32 = 0;
	if (!drive_insert (drv, p, num, p->df[num]))
		return 1;
	if (!drv->diskfile)
		return 1;
	*crc32 = zfile_crc32 (drv->diskfile);
	decode_buffer (drv->bigmfmbuf, drv->cyl, 11, drv->ddhd, drv->filetype, &drvsec, sectable, 1);
	if (sectable[0] == 0 || sectable[1] == 0) {
		ret = 2;
		goto end;
	}
	crc = crc2 = 0;
	for (i = 0; i < 1024; i += 4) {
		uae_u32 v = (writebuffer[i] << 24) | (writebuffer[i + 1] << 16) | (writebuffer[i + 2] << 8) | writebuffer[i + 3];
		if (i == 0)
			dos = v;
		if (i == 4) {
			crc2 = v;
			v = 0;
		}
		if (crc + v < crc)
			crc++;
		crc += v;
	}
	if (dos == 0x4b49434b) { /* KICK */
		ret = 10;
		goto end;
	}
	crc ^= 0xffffffff;
	if (crc != crc2) {
		ret = 3;
		goto end;
	}
	if (dos == 0x444f5300)
		ret = 10;
	else if (dos == 0x444f5301 || dos == 0x444f5302 || dos == 0x444f5303)
		ret = 11;
	else if (dos == 0x444f5304 || dos == 0x444f5305 || dos == 0x444f5306 || dos == 0x444f5307)
		ret = 12;
	else
		ret = 4;
end:
	drive_image_free (drv);
	if (wasdelayed > 1) {
		drive_eject (drv);
		currprefs.df[num][0] = 0;
		drv->dskchange_time = wasdelayed;
		disk_insert (num, drv->newname);
	}
	return ret;
}


/* Disk save/restore code */

#if defined SAVESTATE || defined DEBUGGER

void DISK_save_custom (uae_u32 *pdskpt, uae_u16 *pdsklength, uae_u16 *pdsksync, uae_u16 *pdskbytr)
{
	if(pdskpt) *pdskpt = dskpt;
	if(pdsklength) *pdsklength = dsklength;
	if(pdsksync) *pdsksync = dsksync;
	if(pdskbytr) *pdskbytr = dskbytr_val;
}

#endif /* SAVESTATE || DEBUGGER */

#ifdef SAVESTATE

void DISK_restore_custom (uae_u32 pdskpt, uae_u16 pdsklength, uae_u16 pdskbytr)
{
	dskpt = pdskpt;
	dsklength = pdsklength;
	dskbytr_val = pdskbytr;
}

void restore_disk_finish (void)
{
	setamax();
}

uae_u8 *restore_disk (int num,uae_u8 *src)
{
	drive *drv;
	int state, dfxtype;
	TCHAR old[MAX_DPATH];
	TCHAR *s;
	int newis;

	drv = &floppy[num];
	disabled &= ~(1 << num);
	drv->drive_id = restore_u32 ();
	drv->motoroff = 1;
	drv->idbit = 0;
	state = restore_u8 ();
	if (state & 2) {
		disabled |= 1 << num;
		if (changed_prefs.nr_floppies > num)
			changed_prefs.nr_floppies = num;
		changed_prefs.dfxtype[num] = -1;
	} else {
		drv->motoroff = (state & 1) ? 0 : 1;
		drv->idbit = (state & 4) ? 1 : 0;
		if (changed_prefs.nr_floppies < num)
			changed_prefs.nr_floppies = num;
		switch (drv->drive_id)
		{
		case DRIVE_ID_35HD:
			dfxtype = DRV_35_HD;
			break;
		case DRIVE_ID_525SD:
			dfxtype = DRV_525_SD;
			break;
		default:
			dfxtype = DRV_35_DD;
			break;
		}
		changed_prefs.dfxtype[num] = dfxtype;
	}
	drv->indexhackmode = 0;
	if (num == 0 && currprefs.dfxtype[num] == 0)
		drv->indexhackmode = 1;
	drv->buffered_cyl = -1;
	drv->buffered_side = -1;
	drv->cyl = restore_u8 ();
	drv->dskready = restore_u8 ();
	drv->drive_id_scnt = restore_u8 ();
	drv->mfmpos = restore_u32 ();
	drv->dskchange = 0;
	drv->dskchange_time = 0;
	restore_u32 ();
	s = restore_string ();
	_tcscpy (old, currprefs.df[num]);
	_tcsncpy (changed_prefs.df[num], s, 255);
	xfree (s);
	newis = changed_prefs.df[num][0] ? 1 : 0;
	if (!(disabled & (1 << num))) {
		if (!newis) {
			drv->dskchange = 1;
		} else {
			drive_insert (floppy + num, &currprefs, num, changed_prefs.df[num]);
			if (drive_empty (floppy + num)) {
				if (newis && old[0]) {
					_tcscpy (changed_prefs.df[num], old);
					drive_insert (floppy + num, &currprefs, num, changed_prefs.df[num]);
					if (drive_empty (floppy + num))
						drv->dskchange = 1;
				}
			}
		}
	}
	reset_drive_gui (num);
	return src;
}

static uae_u32 getadfcrc (drive *drv)
{
	uae_u8 *b;
	uae_u32 crc32;
	int size;

	if (!drv->diskfile)
		return 0;
	zfile_fseek (drv->diskfile, 0, SEEK_END);
	size = zfile_ftell (drv->diskfile);
	b = xmalloc (uae_u8, size);
	if (!b)
		return 0;
	zfile_fseek (drv->diskfile, 0, SEEK_SET);
	zfile_fread (b, 1, size, drv->diskfile);
	crc32 = get_crc32 (b, size);
	free (b);
	return crc32;
}

uae_u8 *save_disk (int num, int *len, uae_u8 *dstptr)
{
	uae_u8 *dstbak,*dst;
	drive *drv;

	drv = &floppy[num];
	if (dstptr)
		dstbak = dst = dstptr;
	else
		dstbak = dst = xmalloc (uae_u8, 2+1+1+1+1+4+4+256);
	save_u32 (drv->drive_id);	    /* drive type ID */
	save_u8 ((drv->motoroff ? 0:1) | ((disabled & (1 << num)) ? 2 : 0) | (drv->idbit ? 4 : 0) | (drv->dskchange ? 8 : 0));
	save_u8 (drv->cyl);		    /* cylinder */
	save_u8 (drv->dskready);	    /* dskready */
	save_u8 (drv->drive_id_scnt);   /* id mode position */
	save_u32 (drv->mfmpos);	    /* disk position */
	save_u32 (getadfcrc (drv));	    /* CRC of disk image */
	save_string (currprefs.df[num]);/* image name */

	*len = dst - dstbak;
	return dstbak;
}

/* internal floppy controller variables */

uae_u8 *restore_floppy (uae_u8 *src)
{
	word = restore_u16();
	bitoffset = restore_u8();
	dma_enable = restore_u8();
	disk_hpos = restore_u8() << 8;
	dskdmaen = restore_u8();
	restore_u16 ();
	//word |= restore_u16() << 16;

	return src;
}

uae_u8 *save_floppy(int *len, uae_u8 *dstptr)
{
	uae_u8 *dstbak, *dst;

	/* flush dma buffer before saving */
#if 0
	dodmafetch();
#endif
	if (dstptr)
		dstbak = dst = dstptr;
	else
		dstbak = dst = xmalloc (uae_u8, 2 + 1 + 1 + 1 + 1 + 2);
	save_u16 (word);		/* current fifo (low word) */
	save_u8 (bitoffset);	/* dma bit offset */
	save_u8 (dma_enable);	/* disk sync found */
	save_u8 (disk_hpos >> 8);	/* next bit read position */
	save_u8 (dskdmaen);		/* dma status */
	save_u16 (0);		/* was current fifo (high word), but it was wrong???? */

	*len = dst - dstbak;
	return dstbak;
}

#endif /* SAVESTATE */

#define MAX_DISKENTRIES 4
int disk_prevnext_name (TCHAR *imgp, int dir)
{
	TCHAR img[MAX_DPATH], *ext, *p, *p2, *ps, *dst[MAX_DISKENTRIES];
	int num = -1;
	int cnt, i;
	TCHAR imgl[MAX_DPATH];
	int ret, gotone, wrapped;
	TCHAR *old;

	old = my_strdup (imgp);

	struct zfile *zf = zfile_fopen (imgp, L"rb", ZFD_NORMAL);
	if (zf) {
		_tcscpy (img, zfile_getname (zf));
		zfile_fclose (zf);
		zf = zfile_fopen (img, L"rb", ZFD_NORMAL);
		if (!zf) // oops, no directory support in this archive type
			_tcscpy (img, imgp);
		zfile_fclose (zf);
	} else {
		_tcscpy (img, imgp);
	}

	wrapped = 0;
retry:
	_tcscpy (imgl, img);
	to_lower (imgl, sizeof imgl / sizeof (TCHAR));
	gotone = 0;
	ret = 0;
	ps = imgl;
	cnt = 0;
	dst[cnt] = NULL;
	for (;;) {
		// disk x of y
		p = _tcsstr (ps, L"(disk ");
		if (p && _istdigit (p[6])) {
			p2 = p - imgl + img;
			num = _tstoi (p + 6);
			dst[cnt++] = p2 + 6;
			if (cnt >= MAX_DISKENTRIES - 1)
				break;
			gotone = 1;
			ps = p + 6;
			continue;
		}
		if (gotone)
			 break;
		p = _tcsstr (ps, L"disk");
		if (p && _istdigit (p[4])) {
			p2 = p - imgl + img;
			num = _tstoi (p + 4);
			dst[cnt++] = p2 + 4;
			if (cnt >= MAX_DISKENTRIES - 1)
				break;
			gotone = 1;
			ps = p + 4;
			continue;
		}
		if (gotone)
			 break;
		ext = _tcsrchr (ps, '.');
		if (!ext || ext - ps < 4)
			break;
		TCHAR *ext2 = ext - imgl + img;
		// name_<non numeric character>x.ext
		if (ext[-3] == '_' && !_istdigit (ext[-2]) && _istdigit (ext[-1])) {
			num = _tstoi (ext - 1);
			dst[cnt++] = ext2 - 1;
		// name_x.ext, name-x.ext, name x.ext
		} else if ((ext[-2] == '_' || ext[-2] == '-' || ext[-2] == ' ') && _istdigit (ext[-1])) {
			num = _tstoi (ext - 1);
			dst[cnt++] = ext2 - 1;
		// name_a.ext, name-a.ext, name a .ext
		} else if ((ext[-2] == '_' || ext[-2] == '-' || ext[-2] == ' ') && ext[-1] >= 'a' && ext[-1] <= 'z') {
			num = ext[-1] - 'a' + 1;
			dst[cnt++] = ext2 - 1;
		// nameA.ext
		} else if (ext2[-2] >= 'a' && ext2[-2] <= 'z' && ext2[-1] >= 'A' && ext2[-1] <= 'Z') {
			num = ext[-1] - 'a' + 1;
			dst[cnt++] = ext2 - 1;
		// namex.ext
		} else if (!_istdigit (ext2[-2]) && _istdigit (ext[-1])) {
			num = ext[-1] - '0';
			dst[cnt++] = ext2 - 1;
		}
		break;
	}
	dst[cnt] = NULL;
	if (num <= 0 || num >= 19)
		goto end;
	num += dir;
	if (num > 9)
		goto end;
	if (num == 9)
		num = 1;
	else if (num == 0)
		num = 9;
	for (i = 0; i < cnt; i++) {
		if (!_istdigit (dst[i][0])) {
			int capital = dst[i][0] >= 'A' && dst[i][0] <= 'Z';
			dst[i][0] = (num - 1) + (capital ? 'A' : 'a');
		} else {
			dst[i][0] = num + '0';
		}
	}
	if (zfile_exists (img)) {
		ret = 1;
		goto end;
	}
	if (gotone) { // was (disk x but no match, perhaps there are extra tags..
		TCHAR *old2 = my_strdup (img);
		for (;;) {
			ext = _tcsrchr (img, '.');
			if (!ext)
				break;
			if (ext == img)
				break;
			if (ext[-1] != ']')
				break;
			TCHAR *t = _tcsrchr (img, '[');
			if (!t)
				break;
			t[0] = 0;
			if (zfile_exists (img)) {
				ret = 1;
				goto end;
			}
		}
		_tcscpy (img, old2);
		xfree (old2);
	}
	if (!wrapped) {
		for (i = 0; i < cnt; i++) {
			if (!_istdigit (dst[i][0]))
				dst[i][0] = dst[i][0] >= 'A' && dst[i][0] <= 'Z' ? 'A' : 'a';
			else
				dst[i][0] = '1';
			if (dir < 0)
				dst[i][0] += 8;
		}
		wrapped++;
	}
	if (zfile_exists (img)) {
		ret = -1;
		goto end;
	}
	if (dir < 0 && wrapped < 2)
		goto retry;
	_tcscpy (img, old);

end:
	_tcscpy (imgp, img);
	xfree (old);
	return ret;
}

int disk_prevnext (int drive, int dir)
{
	TCHAR img[MAX_DPATH];

	 _tcscpy (img, currprefs.df[drive]);

	if (!img[0])
		return 0;
	disk_prevnext_name (img, dir);
	_tcscpy (changed_prefs.df[drive], img);
	return 1;
}


