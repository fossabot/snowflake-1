﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowflake.Stone.FileSignatures.Formats.CDXA
{
    internal class CDXABlockStream : Stream
    {
        private readonly Stream diskStream;
        public const long BlockLength = CDXADisc.BlockSize - CDXADisc.BlockHeaderSize;

        public CDXABlockStream(uint lba, Stream diskStream)
        {
            this.diskStream = diskStream;
            this.LBA = lba;
            this.Length = CDXABlockStream.BlockLength;
        }

        /// <inheritdoc/>
        public sealed override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.Length + offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
            }

            return this.Position;
        }

        /// <inheritdoc/>
        public sealed override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Does not protect against out of bounds reading.
        /// To access file contents, use the much safer <see cref="CDXAFile.OpenFile"/>
        /// </remarks>
        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            this.diskStream.Seek((CDXADisc.BlockSize * this.LBA) + CDXADisc.BlockHeaderSize + this.Position,
                SeekOrigin.Begin);
            this.Position += count;
            return this.diskStream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public sealed override bool CanRead => true;

        /// <inheritdoc/>
        public sealed override bool CanSeek => true;

        /// <inheritdoc/>
        public sealed override bool CanWrite => false;

        /// <inheritdoc/>
        public sealed override long Length { get; }

        public uint LBA { get; }

        /// <inheritdoc/>
        public sealed override long Position { get; set; }
    }
}
