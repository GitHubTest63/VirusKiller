using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushroomsUnity3DExample.Messages
{
    class PositionMessage : AbstractMessage
    {
        private string name;
        private float posX;
        private float posY;
        private float posZ;


        public PositionMessage(string name, float posX, float posY, float posZ)
            : base("PositionMessage", name, posX, posY, posZ)
        {
            this.name = name;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
        }
    }
}
