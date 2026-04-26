using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();
            ModbusReadCommandParameters p =
        this.CommandParameters as ModbusReadCommandParameters;

            byte[] request = new byte[12];

            request[0] = (byte)(p.TransactionId >> 8);
            request[1] = (byte)(p.TransactionId & 0xFF);

            request[2] = 0;
            request[3] = 0;

            request[4] = 0;
            request[5] = 6;

            request[6] = p.UnitId;
            request[7] = (byte)ModbusFunctionCode.READ_INPUT_REGISTERS;

            request[8] = (byte)(p.StartAddress >> 8);
            request[9] = (byte)(p.StartAddress & 0xFF);

            request[10] = (byte)(p.Quantity >> 8);
            request[11] = (byte)(p.Quantity & 0xFF);

            return request;
        }
        /*
        //mozda ispravnije
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
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ModbusReadCommandParameters p =
                this.CommandParameters as ModbusReadCommandParameters;

            ushort startAddress = p.StartAddress;

            byte byteCount = response[8];
            int responseIndex = 9;

            for (int i = 0; i < byteCount / 2; i++)
            {
                byte high = response[responseIndex++];
                byte low = response[responseIndex++];

                ushort value = (ushort)((high << 8) | low);
                ushort address = (ushort)(startAddress + i);

                result.Add(
                    new Tuple<PointType, ushort>(
                        PointType.ANALOG_INPUT,
                        address),
                    value);
            }

            return result;
        }
        //mozda ispravnije
        /*
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
{
    Dictionary<Tuple<PointType, ushort>, ushort> resp = new Dictionary<Tuple<PointType, ushort>, ushort>();
    if ((response[7] & 0x80) == 0)
    {
        int byteCount = response[8];
        int quantity = ((ModbusReadCommandParameters)CommandParameters).Quantity;
        ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
        for (int i = 0; i < byteCount/2; i++)
        {
            var value = (ushort)(response[9 + i * 2] << 8 | response[10 + i * 2]);
            resp.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, (ushort)(startAddress + i)), value);
        }
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
