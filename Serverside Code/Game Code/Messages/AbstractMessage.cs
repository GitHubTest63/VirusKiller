using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample.Messages
{
    class AbstractMessage
    {
        Message msg;

        public AbstractMessage(string msgType, params Object[] values)
        {
            this.msg = Message.Create(msgType, values);
        }

        public Message getPreparedMessage()
        {
            return this.msg;
        }
    }
}
