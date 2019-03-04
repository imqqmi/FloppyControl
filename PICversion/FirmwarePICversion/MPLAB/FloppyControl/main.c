/*
 * File:   main.c
 * Author: Josha
 * PIC16F1827
 * Created on 26 juli 2015, 13:50
 * LabPSU v1
 */

// Pins 27/28 = RB6/RB7 = DAT/CLK, not to be used for other purposes.
// pins 17/RC6 = TX, pin 18/RC7 = RX

#include <xc.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#define _XTAL_FREQ 80000000
#define FCY    _XTAL_FREQ / 4

#define BAUDRATE 5000000
#define BRGVAL ((FCY/BAUDRATE)/4)-1


// #pragma config statements should precede project file includes.
// Use project enums instead of #define for ON and OFF.

// CONFIG1
#pragma config FOSC = ECH       // Oscillator Selection (INTOSC oscillator: I/O function on CLKIN pin)
#pragma config PLLEN = ON       // Phase locked loop off
#pragma config WDTE = SWDTEN    // Watchdog Timer Enable (WDT disabled)
#pragma config PWRTE = OFF      // Power-up Timer Enable (PWRT disabled)
#pragma config MCLRE = OFF      // MCLR Pin Function Select (MCLR/VPP pin function is digital input)
#pragma config CP = OFF         // Flash Program Memory Code Protection (Program memory code protection is disabled)
#pragma config CPD = OFF        // Data Memory Code Protection (Data memory code protection is disabled)
#pragma config BOREN = OFF      // Brown-out Reset Enable (Brown-out Reset disabled)
#pragma config CLKOUTEN = OFF   // Clock Out Enable (CLKOUT function is disabled. I/O or oscillator function on the CLKOUT pin)
#pragma config IESO = OFF       // Internal/External Switchover (Internal/External Switchover mode is disabled)
#pragma config FCMEN = OFF      // Fail-Safe Clock Monitor Enable (Fail-Safe Clock Monitor is disabled)

// CONFIG2
#pragma config WRT = OFF        // Flash Memory Self-Write Protection (Write protection off)
//#pragma config PLLEN = OFF       // PLL Enable (4x PLL enabled)
#pragma config STVREN = ON      // Stack Overflow/Underflow Reset Enable (Stack Overflow or Underflow will cause a Reset)
#pragma config BORV = LO        // Brown-out Reset Voltage Selection (Brown-out Reset Voltage (Vbor), low trip point selected.)
#pragma config LVP = ON         // Low-Voltage Programming Enable (Low-voltage programming enabled)

#define IN              1
#define OUT             0

// Outputs
#define MOTEB           LATC0
#define DRVSB           LATC1
#define MOTEA           LATC5


#define DIR             LATC2
#define STEP            LATC3
#define SIDE            LATC4
#define SELECT0         LATC5
#define _STEPSTICKEN    LATA6
#define MS1             LATA2 
#define MS2             LATA1 
#define MS3             LATA0 
#define WDATE           LATB5
#define WGATE           LATB4
#define RESET           LATA4

//Inputs, IOC
#define RDATA           PORTBbits.RB0   
#define RDATAIE         INTE
#define TRK00           PORTBbits.RB1
#define INDEX           PORTBbits.RB2   
#define READY           PORTAbits.RA3
//#define INUSE           PORTAbits.RA4


#define DATASIZE        256

#define TRACKDELAY      3 //in ms

//unsigned char d[3];
//unsigned short tempbuf[10];
char recvbuf;
char recvchar=0;
char data[DATASIZE];
char buf[10];
char index=0;
char indexchanged = 0;
char trk00 = 0;
char curtrk = 0; // an attempt at keeping track of the current track
unsigned long rdatacount = 0; // I used this to try to count the pulses but longs took too long ;)
char tmr = 0; // stores the timer0 value at the start of the isr
char getdata =0; // instructs the ISR to start capturing the data.
volatile unsigned char datacnt = 0; // index for the data[] buffer, in ram for now.
char readbyte = 0; // holds the current byte we're reading
char readbytecnt = 0; // counts pairs of bits so we know when we've got a full byte
char readerr = 0;
char indexgo = 0;
char block=1;
char blockreset = 1;
char dumpdatadone = 0;
volatile char stop =0;
char microstep = 8;

// Function definitions

// System
InitOSC();
void menu(void);
void interrupt ISR(void);

// Floppy Control
void dumpdata(void);
void seektrk00(void);
void gototrack(char t);
void inctrack(void);
void setMS();

// Serial
void numtostr( char *b, char n);
void numtohex( char *b, char n);
void txhex( char n);
void txnum( char n);
void txstring(const char *data);
void serialWaitForReady(void);
void TxtDisplay(unsigned char *t);

void main(void) {
    char q;
    
    WDTCONbits.SWDTEN = 1;
    
    CLRWDT();
    WDTCON = 0b0001011001;
    
    
    INTCON = 0; // disable all interrupts. 
    //WDTCON = 0;
    PEIE = 0;
    
    CPSON = 0; // Disable touch
    SRLEN = 0; // Disable SR-Latch
    
    LATC = 0xff;
    LATB = 0x00;
	LATA = 0b01000000; //LATA6 = _STEPSTICKEN.
	
    //LATAbits.LATA6 = 1;
    
	TRISA = 0b00000000; // All outputs
	TRISB = 0b00001111; // Bits 0..3 input for interrupt on change, others are output
    TRISC = 0b10000000;
    
    CPSCON0 = 0;
    //C0ON = 0;
    C1ON = 0;
    
    ADCON0 = 0b00000000;
    ADCON1 = 0b00000000;
    FVRCON = 0b00000000;
    
    ANSELA = 0b00000000;
	ANSELB = 0b00000000;
    
	OPTION_REG = 0b11111000;
	
	WPUB = 0b00000000;
	
    //WPUA = 0b00000000;
    //InitOSC();
    
    short cnt=0;
    unsigned short temp;
    short tempcnt=0;
    unsigned short voutsp = 0, voutlsb;
    int i;
    unsigned char x; // tempbuf counter

    //Setup serial
    // pin 17/RC6 = TX, pin 18/RC7 = RX
    SPBRGH = 0;
    SPBRGL = BRGVAL;    // For 32Mhz and baudrate 117600
    SYNC = 0;       // Setup for asynchronous comm.
    BRGH = 1;       // High speed baudrate
    BRG16 = 0;      // 8 bit baudrate generator used
    
    //Enable serial port
    SYNC = 0;
    SPEN = 1;
    
    //Enable transmit
    TXEN = 1;
    RCIE = 1; // Enable serial receive interrupt
    
    
    //Setup interrupt on change pins
    IOCBFbits.IOCBF2 = 0;
    IOCIE = 1; // Enable interrupt on change on PORTB  
    // Remember all signals are inverted as seen from the floppy drive!
    // RB3 = DSKRDY, RB2 = RDATA, RB1 = TRK00, RB0 = index
    IOCBN = 0b00000100; // Falling edge Index
    IOCBP = 0b00000000; // Rising edge
    //IOCBN2 = 0;

    RDATAIE = 0;
    INTEDG = 0; // falling edge, which is leading in the case of an inverted signal
    
    CREN = 1; // Enable serial reception

    //Timer0 setup for measuring the time between pulses
    OPTION_REG = 0b00001000;

    //Capture configuration
    //CCP1SEL = 1; //CCP1 on RB0/RDATA
    CCP4CON = 0b00000100; // Capture every falling edge = 0b0000100, rising = 0b0000101
    PIE3bits.CCP4IE = 0;
    PIR3bits.CCP4IF = 0;
    
    //CCP1CONbits.
    T1CON = 0x00;
    T1CONbits.TMR1CS = 0b00; // Clock source instruction clock (FOSC/4)
    T1CONbits.T1CKPS = 0b00; // prescale 1:1, instruction cycle
    T1CONbits.nT1SYNC = 0b0; // Syncronized with osc
    T1CONbits.TMR1ON = 0b1; // Timer on
    T1GCON = 0x00; // Gate control off
    
    PEIE = 1;
    GIE  = 1; // Enable global interrupts
    
    //__delay_ms(100);
    txstring("Test Floppy 01\r\n");
    
    //LATA = 0b00000011; // Setup microstepping 8x
    
    MS1 = 1; // microstepping 8x
    MS2 = 1;
    MS3 = 0;
    
    WGATE = 1;
    WDATE = 1;

    stop = 1;
    while(1) 
    {
        CLRWDT();
        if( stop )
        {
            PIE3bits.CCP4IE = 0;
        }
        else
        {
            PIE3bits.CCP4IE = 1;
        }
    }
    
}

void menu(void)
{
    char step;
    
    if( recvchar )
    {
        recvchar = 0;
        switch(recvbuf)
        {

            case 'q': // Deselect drive
                //txstring("x12 DRVSB enabled\r\n\0");
                DRVSB = 1;
                SELECT0 = 1;
                RESET = 0;
            break;
            case 'w': // Disable  motor
                //txstring("16 MOTEB enabled\r\n");
                MOTEB = 1;
                _STEPSTICKEN = 1;
            break; 
            case 'e': // DIR = 1
                //txstring("DIR enabled\r\n");
                DIR = 1;
            break;
            case 'r':
                //txstring("STEP enabled\r\n");
                STEP = 1;
            break;                
//**************************************************************
            case 'a': // Select drive
                //txstring("12 DRVSB disabled\r\n");
                DRVSB = 0;
                SELECT0 = 1;
                RESET = 1;
            break;
            case 's': // Enable motor and stepstick
                //txstring("16 MOTEB disabled\r\n");
                MOTEB = 0;
                MOTEA = 0;
                _STEPSTICKEN = 0;
            break;
            case 'd': // DIR = 0
                //txstring("DIR disabled\r\n");
                DIR = 0;
            break;
            case 'f':
                //txstring("STEP disabled\r\n");
                STEP = 0;
            break;
//***************************************************************            
            case '0': // Seek track 00
                //txstring("Seeking track 00..\r\n");
                curtrk = 0;
                seektrk00();
            break;
            case '8': // Goto track 80
                //txstring("Goto track 80..\r\n");
                gototrack(80);
            break;
            case 'v': // dump data to console
                txstring("Dumping data..\r\n");
                txnum(CCPR4L);
                txnum(CCPR4H);
                //dumpdata();
            break;
            case 't': // next track
                curtrk++;
                inctrack();
            break;
             case 'y': // next track
                curtrk+=microstep;
                for( step = 0; step < microstep; step++)
                    inctrack();
            break;           
            case 'g': // previous track
                curtrk--;
                DIR = 1; // decrement
    
                STEP = 0; // pulse a step, 2us pulses
                __delay_us(6);
                STEP = 1;
                __delay_ms(TRACKDELAY); // wait 20ms between pulses
                CLRWDT();
            break;
            case '.': // 
                // stop received
                //txstring("Stop=1\r\n");
                stop = 1;

            break;
            case ',': // Continue capture
                // stop received
                //txstring("Stop=0\r\n\0");
                stop = 0;

            break;
            case 'h': // HEAD 0
                
                SIDE = 1;

            break;
            case 'j': // HEAD 1
                
                SIDE = 0;

            break;
            case '[':
                microstep++;
                setMS();
                break;
            case ']':
                microstep = 1;
                setMS();
                break;
            
            case 'i':
                txstring("PIC16F1938 working!! TRK00: ");
                //txstring(" TRK00:");
                txnum(TRK00);
                txstring("\r\n");
                break;
            default:
                break;
        }
    }
}

// Set microstepping depending on the user chonsen microstep
void setMS()
{
    unsigned char ms;
    switch(microstep)
    {
        case 1:
            ms = 0b000;
            break;
        case 2:
            ms = 0b011;
            break;
        case 4:
            ms = 0b010;
            break;
        case 8:
            ms = 0b110;
            break;
        case 16:
            ms = 0b111;
            break;

        default: 
            ms = 0b000; // full step
            break;
    }
    
    MS1 = (ms & 0b100) >> 2;
    MS2 = (ms & 0b010) >> 1;
    MS3 = (ms & 0b001);
    
}

void interrupt ISR(void)
{
    //static short ccpold, ccp;
    
    //static unsigned char ccpl,ccph;
    if( PIR3bits.CCP4IF)
    {
        TMR1L = 0;
        TMR1H = 0;
        if( CCPR4L > 3)
            TXREG = CCPR4L;

        PIR3bits.CCP4IF = 0;
        
    }    
    /*
    if( PIR3bits.CCP4IF)
    {
        //TMR1L = 0;
        //TMR1H = 0;
        
        ccp = CCPR4 - ccpold;
        
        ccpold = CCPR4;
        
        if( ccp > 255 ) ccp = 255;
        
        if( ccp > 3)
            TXREG = ccp;

        PIR3bits.CCP4IF = 0;
        
    }
    */
    if( IOCBFbits.IOCBF2) //Index signal detected
    {
        if( stop == 0) // only send index signals if capturing
        TXREG = 0x01; // 0x01 means index signal detected
        IOCBFbits.IOCBF2=0;
        
    }
    if( RCIF )
    {
        recvchar = 1;
        recvbuf = RCREG;
        menu();
    } 
    
}

void dumpdata(void)
{
    unsigned int i;
    unsigned char j;
    
    //for(i=DATASIZE; i>0; i--)
    //txstring("\r\n");
    j=7;
    for(i=0; i<DATASIZE; i++)
    {
        j = (j + 1) % DATASIZE;
        //txnum(j);
        //txstring(",");
        
        txhex(data[j]);
        txstring(",");
    }
    //txhex(data[i]);
    //txstring(",");
    //txstring("\r\n");
    dumpdatadone = 1;
    
}

// Seek TRK00
void seektrk00(void) 
{
    int i,j;
    
    DIR = 1; // decrement until trk00 is found
    
    // Set the microstepping to full step
    MS1 = 0;
    MS2 = 0;
    MS3 = 0;
    
    // Quick seek for speed
    for( i=0; i<180; i++)
    {
        if( !TRK00 ) //If the interrupt triggered, exit the loop
            break;
        
        STEP = 0; // pulse a step, 10 us pulses
        __delay_us(20);
        STEP = 1;
        __delay_ms(3); // wait 20ms between pulses
        CLRWDT();
    }
    
    // move head forward again
    DIR = 0;
    for( i=0; i<10; i++)
    {
        STEP = 0; // pulse a step, 2us pulses
        __delay_us(20); 
        STEP = 1;
        CLRWDT();
        __delay_ms(3); 
    }

    // Seek slowly for accuracy
    DIR = 1;
    for( i=0; i<250; i++)
    {
        if( !TRK00 ) //If the interrupt triggered, exit the loop
            break;
        
        STEP = 0; // pulse a step, 10 us pulses
        __delay_us(100);
        STEP = 1;
        __delay_ms(15); // wait 20ms between pulses
        CLRWDT();
    }

    // return the microstepping to the user set value
    setMS();
}

// Go to track t, seeking first track 00
void gototrack(char t)
{
    char i,j;
    
    unsigned int number_of_steps;
    unsigned int cnt;

    DIR = 1; // decrement until trk00 is found
    
    // Home head to track 00
    seektrk00();
    MS1 = 0;
    MS2 = 0;
    MS3 = 0;
    
    DIR = 0;
    // Go to requisted track
    
    number_of_steps = t * 2;
    for( cnt = 0; cnt <number_of_steps; cnt++)
    {
        
        STEP = 0; // pulse a step, 2us pulses
        __delay_ms(1); 
        STEP = 1;
        CLRWDT();
        __delay_ms(2); 
    }
    
    setMS();

}

// Increment 1 track, assuming the motor is spinning
void inctrack(void)
{
    DIR = 0;
    // Go to requested track
        
    STEP = 0; // pulse a step, 2us pulses
    __delay_us(6);
    STEP = 1;
    __delay_ms(TRACKDELAY); // wait 20ms between pulses
    CLRWDT();
}

// Converts a char to a string
void numtostr( char *b, char n)
{
	n=n % 1000;
	b[0] = (n / 100) + 48;

	n=n % 100;
	b[1] = (n / 10) + 48;

	n=n % 10;
	b[2] = n + 48;
    
    b[3] = 0;
}

// Converts a char to a hex string
void numtohex( char *b, char n)
{
	const char hex[] = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
    
    buf[0] = hex[ n >> 4];
    buf[1] = hex[ n & 0b1111];
    buf[2] = 0;
    
}

void txhex( char n)
{
    numtohex( buf, n);
    txstring(buf);
}

void txnum( char n)
{
    numtostr( buf, n);
    txstring(buf);
}


// Send a string to serial io, waiting for each bit to be sent.
void txstring(const char *data)
{
    int y;
    
    for(y = 0; y < 
            strlen( data ); 
            y++)
    {
       TXREG = data[y];
       serialWaitForReady();
    }
    
}

void serialWaitForReady(void)
{
    while(!TRMT) {;} // wait for bits to be sent
    //__delay_us(500);
}

InitOSC()
{
	OSCCON = 0b11110000; // PLL_EN, 8MHz/32Mhz, FOSC=config
}


