using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushroomsUnity3DExample
{
    class ByteReader
    {
        private byte[] data;
        private int index;

        public ByteReader(byte[] data)
        {
            this.data = data;
            this.index = 0;
        }

        public float readFloat()
        {
            float f = BitConverter.ToSingle(this.data, this.index);
            this.index += 4;
            return f;
        }
    }
}
