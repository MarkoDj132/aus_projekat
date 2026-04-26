using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();
            ModbusWriteCommandParameters p =
         this.CommandParameters as ModbusWriteCommandParameters;

            byte[] request = new byte[12];

            request[0] = (byte)(p.TransactionId >> 8);
            request[1] = (byte)(p.TransactionId & 0xFF);

            request[2] = 0;
            request[3] = 0;

            request[4] = 0;
            request[5] = 6;

            request[6] = p.UnitId;
            request[7] = (byte)ModbusFunctionCode.WRITE_SINGLE_COIL;

            request[8] = (byte)(p.OutputAddress >> 8);
            request[9] = (byte)(p.OutputAddress & 0xFF);

            ushort value = p.Value == 0 ? (ushort)0x0000 : (ushort)0xFF00;

            request[10] = (byte)(value >> 8);
            request[11] = (byte)(value & 0xFF);

            return request;
        }
        //mozda ispravnije
        /*
        public override byte[] PackRequest()
{
    byte[] ret_val = new byte[12];

    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.TransactionId)), 0, ret_val, 0, 2);
    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.ProtocolId)), 0, ret_val, 2, 2);
    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)CommandParameters.Length)), 0, ret_val, 4, 2);
    ret_val[6] = CommandParameters.UnitId;
    ret_val[7] = CommandParameters.FunctionCode;
    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((ModbusReadCommandParameters)CommandParameters).StartAddress)), 0, ret_val, 8, 2);
    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(((ModbusReadCommandParameters)CommandParameters).Quantity))), 0, ret_val, 10, 2);

    return ret_val;
}
        */
        

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();
            //throw new Exception("WRITE RESPONSE RECEIVED");
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            byte functionCode = response[7];

            if ((functionCode & 0x80) != 0)
            {
                HandeException(response[8]);
            }

            ushort address =
                (ushort)((response[8] << 8) | response[9]);

            ushort value =
                (response[10] == 0xFF) ? (ushort)1 : (ushort)0;

            result.Add(
                new Tuple<PointType, ushort>(
                    PointType.DIGITAL_OUTPUT,
                    address),
                value);

            return result;
        }
        //mozda ispravnije
        /*
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
{
    Dictionary<Tuple<PointType, ushort>, ushort> resp = new Dictionary<Tuple<PointType, ushort>, ushort>();
    if (response[7] != CommandParameters.FunctionCode + 0x80)
    {
        var address = BitConverter.ToUInt16(response, 8);
        var value = BitConverter.ToUInt16(response, 10);

        address = (ushort)IPAddress.NetworkToHostOrder((short)address);
        value = (ushort)IPAddress.NetworkToHostOrder((short)value);

        resp.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address), value);
    }
    else
    {
        HandeException(response[8]);
    }

    return resp;
}
        */
    }
}
