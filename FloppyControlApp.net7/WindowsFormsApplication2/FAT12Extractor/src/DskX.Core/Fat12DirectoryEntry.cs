using System.Text;

namespace DskX.Core;

/// <summary>
/// Represents a FAT12 directory entry (32 bytes).
/// Internal structure used for parsing directories.
/// </summary>
internal class Fat12DirectoryEntry
{
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public byte Attributes { get; set; }
    public ushort StartCluster { get; set; }
    public uint FileSize { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }
    
    // Attribute flags
    public const byte ATTR_READ_ONLY = 0x01;
    public const byte ATTR_HIDDEN = 0x02;
    public const byte ATTR_SYSTEM = 0x04;
    public const byte ATTR_VOLUME_LABEL = 0x08;
    public const byte ATTR_DIRECTORY = 0x10;
    public const byte ATTR_ARCHIVE = 0x20;
    
    // Special markers
    public const byte DELETED_MARKER = 0xE5;
    public const byte END_OF_DIRECTORY = 0x00;
    
    public bool IsDirectory => (Attributes & ATTR_DIRECTORY) != 0;
    public bool IsVolumeLabel => (Attributes & ATTR_VOLUME_LABEL) != 0;
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Gets the full name (filename.extension).
    /// </summary>
    public string FullName
    {
        get
        {
            if (string.IsNullOrEmpty(Extension))
                return FileName;
            return $"{FileName}.{Extension}";
        }
    }
    
    /// <summary>
    /// Parses a 32-byte directory entry from a buffer.
    /// </summary>
    /// <param name="buffer">Buffer containing the directory entry.</param>
    /// <param name="offset">Offset within the buffer.</param>
    /// <returns>Parsed directory entry, or null if end of directory or invalid.</returns>
    public static Fat12DirectoryEntry? Parse(byte[] buffer, int offset)
    {
        if (offset + 32 > buffer.Length)
            return null;
        
        byte firstByte = buffer[offset];
        
        // Check for end of directory
        if (firstByte == END_OF_DIRECTORY)
            return null;
        
        var entry = new Fat12DirectoryEntry();
        
        // Check for deleted entry
        if (firstByte == DELETED_MARKER)
        {
            entry.IsDeleted = true;
            // We skip deleted entries for now (Phase 1 limitation)
            return null;
        }
        
        // Parse filename (8 bytes, space-padded)
        entry.FileName = Encoding.ASCII.GetString(buffer, offset, 8).TrimEnd();
        
        // Parse extension (3 bytes, space-padded)
        entry.Extension = Encoding.ASCII.GetString(buffer, offset + 8, 3).TrimEnd();
        
        // Parse attributes
        entry.Attributes = buffer[offset + 11];
        
        // Skip volume labels (they're not files)
        if (entry.IsVolumeLabel)
            return null;
        
        // Parse start cluster (offset 26, 2 bytes)
        entry.StartCluster = ReadUInt16(buffer, offset + 26);
        
        // Parse file size (offset 28, 4 bytes)
        entry.FileSize = ReadUInt32(buffer, offset + 28);
        
        // Parse timestamps (optional, can be zero)
        try
        {
            // Creation date/time (offset 14-17)
            ushort createTime = ReadUInt16(buffer, offset + 14);
            ushort createDate = ReadUInt16(buffer, offset + 16);
            if (createDate != 0)
                entry.Created = DecodeFatDateTime(createDate, createTime);
            
            // Modified date/time (offset 22-23)
            ushort modTime = ReadUInt16(buffer, offset + 22);
            ushort modDate = ReadUInt16(buffer, offset + 24);
            if (modDate != 0)
                entry.Modified = DecodeFatDateTime(modDate, modTime);
        }
        catch
        {
            // Ignore timestamp parsing errors
        }
        
        return entry;
    }
    
    /// <summary>
    /// Decodes FAT date/time format to DateTime.
    /// </summary>
    private static DateTime DecodeFatDateTime(ushort date, ushort time)
    {
        // Date format: bits 0-4 = day, 5-8 = month, 9-15 = year (since 1980)
        int day = date & 0x1F;
        int month = (date >> 5) & 0x0F;
        int year = ((date >> 9) & 0x7F) + 1980;
        
        // Time format: bits 0-4 = seconds/2, 5-10 = minutes, 11-15 = hours
        int seconds = (time & 0x1F) * 2;
        int minutes = (time >> 5) & 0x3F;
        int hours = (time >> 11) & 0x1F;
        
        // Validate ranges
        if (year < 1980 || year > 2107 || month < 1 || month > 12 || day < 1 || day > 31 ||
            hours > 23 || minutes > 59 || seconds > 59)
        {
            return DateTime.MinValue;
        }
        
        try
        {
            return new DateTime(year, month, day, hours, minutes, seconds, DateTimeKind.Utc);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
    
    private static ushort ReadUInt16(byte[] buffer, int offset)
    {
        return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
    }
    
    private static uint ReadUInt32(byte[] buffer, int offset)
    {
        return (uint)(buffer[offset] | 
                     (buffer[offset + 1] << 8) | 
                     (buffer[offset + 2] << 16) | 
                     (buffer[offset + 3] << 24));
    }
}
