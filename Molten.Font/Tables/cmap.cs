﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Font
{
    /// <summary>Horizontal Device Metrics (hdmx) table.<para/>
    /// See: https://docs.microsoft.com/en-us/typography/opentype/spec/hdmx </summary>
    [FontTableTag("cmap")]
    public class Cmap : FontTable
    {
        public ushort Version { get; internal set; }

        public CmapSubTable[] Tables { get; internal set; }

        Dictionary<int, ushort> _charIndexToGlyph = new Dictionary<int, ushort>();

        public ushort LookupIndex(int codepoint, int nextCodepoint = 0)
        {
            // MS Docs: Character codes that do not correspond to any glyph in the font should be mapped to glyph index 0.
            // See: https://www.microsoft.com/typography/OTSPEC/cmap.htm
            ushort result = 0;

            if (!_charIndexToGlyph.TryGetValue(codepoint, out result))
            {
                foreach (CmapSubTable cmap in Tables)
                {
                    ushort glyphID = cmap.CharToGlyphIndex(codepoint);

                    // MS Docs: When building a Unicode font for Windows, the platform ID should be 3 and the encoding ID should be 1.
                    // See: https://www.microsoft.com/typography/OTSPEC/cmap.htm
                    if (result == 0 || (glyphID != 0 && cmap.Platform == FontPlatform.Windows && cmap.Encoding == 1))
                        result = glyphID;
                }

                _charIndexToGlyph[codepoint] = result;
            }

            // If there is a second codepoint, we are asked whether this is an UVS sequence
            //  -> if true, return a glyph ID
            //  -> otherwise, return 0
            if (nextCodepoint > 0)
            {
                foreach (CmapSubTable cmap in Tables)
                {
                    ushort gylphID = cmap.CharPairToGlyphIndex(codepoint, result, nextCodepoint);
                    if (gylphID > 0)
                        return gylphID;
                }

                return 0;
            }

            return result;
        }

        internal override void Read(BinaryEndianAgnosticReader reader, TableHeader header, Logger log, FontTableList dependencies)
        {
            ushort version = reader.ReadUInt16();
            ushort numRecords = reader.ReadUInt16();
            CmapEncodingRecord[] records = new CmapEncodingRecord[numRecords];

            // Read offsets and prepare records.
            for (int i = 0; i < numRecords; i++)
            {
                records[i] = new CmapEncodingRecord()
                {
                    Platform = (FontPlatform)reader.ReadUInt16(),
                    Encoding = reader.ReadUInt16(),
                    Offset = reader.ReadUInt32(),
                };
            }

            Version = version;
                Tables = new CmapSubTable[numRecords];

            // Populate records based on their format
            for (int i = 0; i < numRecords; i++)
            {
                CmapEncodingRecord record = records[i];
                reader.Position = header.Offset + record.Offset;
                record.Format = reader.ReadUInt16();

                switch (record.Format)
                {
                    case 0: Tables[i] = new CmapFormat0SubTable(reader, log, this, record.Offset + 2, record); break;
                    //case 2: ReadFormat2(reader, record); break; // Had no luck finding a font with format_2 cmap subtables. Need one for testing.
                    case 4: Tables[i] = new CmapFormat4SubTable(reader, log, this, record.Offset + 2, record); break;
                    case 6: Tables[i] = new CmapFormat6SubTable(reader, log, this, record.Offset + 2, record); break;
                    default:
                        log.WriteDebugLine($"[CMAP] Unsupported format for sub-table {i}/{numRecords - 1}: Format {record.Format}");
                        break;
                }
            }

            reader.Position = header.Offset + header.Length;
        }
    }


    public class CmapEncodingRecord
    {
        public ushort Format { get; internal set; }

        public FontPlatform Platform { get; internal set; }

        public ushort Encoding { get; internal set; }

        public uint Offset { get; internal set; }
    }
}
