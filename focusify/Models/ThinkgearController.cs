using libStreamSDK;
using System;

namespace focusify
{
    public class ThinkgearController
    {
        public int connect()
        {
            System.Diagnostics.Debug.WriteLine("hello");

            /* Print driver version number */
            System.Diagnostics.Debug.WriteLine("Version: " + NativeThinkgear.TG_GetVersion());

            /* Get a connection ID handle to ThinkGear */
            int connectionID = NativeThinkgear.TG_GetNewConnectionId();
            System.Diagnostics.Debug.WriteLine("Connection ID: " + connectionID);

            if (connectionID < 0)
            {
                throw new SystemException("ERROR: TG_GetNewConnectionId() returned: " + connectionID);
            }

            int errCode = 0;
            // Set/open stream (raw bytes) log file for connection
            string streamLogPath = "C:\\Users\\Mateusz\\focusify\\streamLog.txt";
            errCode = NativeThinkgear.TG_SetStreamLog(connectionID, streamLogPath);
            System.Diagnostics.Debug.WriteLine("errCode for TG_SetStreamLog : " + errCode);
            if (errCode < 0)
            {
                throw new SystemException("ERROR: TG_SetStreamLog() returned: " + errCode);
            }

            // Set/open data (ThinkGear values) log file for connection
            string dataLogPath = "C:\\Users\\Mateusz\\focusify\\dataLog.txt";
            errCode = NativeThinkgear.TG_SetDataLog(connectionID, dataLogPath);
            System.Diagnostics.Debug.WriteLine("errCode for TG_SetDataLog : " + errCode);
            if (errCode < 0)
            {
                throw new SystemException("ERROR: TG_SetDataLog() returned: " + errCode);
            }//*/

            /* Attempt to connect the connection ID handle to serial port "COM5" */
            string comPortName = "\\\\.\\COM6";

            errCode = NativeThinkgear.TG_Connect(connectionID,
                          comPortName,
                          NativeThinkgear.Baudrate.TG_BAUD_57600,
                          NativeThinkgear.SerialDataFormat.TG_STREAM_PACKETS);
            if (errCode < 0)
            {
                throw new SystemException("ERROR: TG_Connect() returned: " + errCode);
            }

            return connectionID;
        }

        public void readPackets(int connectionID, int packetsToRead)
        {
            int errCode;
            /* Read packetsToRead ThinkGear Packets from the connection, 1 Packet at a time */
            int packetsRead = 0;
            while (packetsRead < packetsToRead)
            {

                /* Attempt to read a Packet of data from the connection */
                errCode = NativeThinkgear.TG_ReadPackets(connectionID, 1);
                System.Diagnostics.Debug.WriteLine("TG_ReadPackets returned: " + errCode);
                /* If TG_ReadPackets() was able to read a complete Packet of data... */
                if (errCode == 1)
                {
                    packetsRead++;

                    /* If attention value has been updated by TG_ReadPackets()... */
                    if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_ATTENTION) != 0)
                    {

                        /* Get and print out the updated attention value */
                        System.Diagnostics.Debug.WriteLine("New Attention value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_ATTENTION));

                    } /* end "If attention value has been updated..." */

                    /* If raw value has been updated by TG_ReadPackets()... */
                    if (NativeThinkgear.TG_GetValueStatus(connectionID, NativeThinkgear.DataType.TG_DATA_ALPHA1) != 0)
                    {

                        /* Get and print out the updated raw value */
                        System.Diagnostics.Debug.WriteLine("New raw value: : " + (int)NativeThinkgear.TG_GetValue(connectionID, NativeThinkgear.DataType.TG_DATA_RAW));

                    } /* end "If raw value has been updated..." */

                } /* end "If a Packet of data was read..." */

            } /* end "Read 10 Packets of data from connection..." */

        }

        public void disconnect(int connectionID)
        {
            NativeThinkgear.TG_Disconnect(connectionID);

            /* Clean up */
            NativeThinkgear.TG_FreeConnection(connectionID);
        }
    }
}
