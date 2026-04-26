using Common;
using Modbus.FunctionParameters;//test 
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
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
            request[7] = (byte)ModbusFunctionCode.READ_COILS;

            request[8] = (byte)(p.StartAddress >> 8);
            request[9] = (byte)(p.StartAddress & 0xFF);

            request[10] = (byte)(p.Quantity >> 8);
            request[11] = (byte)(p.Quantity & 0xFF);

            return request;
        }
		//mozda ispravnije
		/*public override byte[] PackRequest()
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
		}*/

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ModbusReadCommandParameters p =
                this.CommandParameters as ModbusReadCommandParameters;

            ushort startAddress = p.StartAddress;
            ushort quantity = p.Quantity;

            byte byteCount = response[8];
            int responseIndex = 9;

            int pointIndex = 0;

            for (int i = 0; i < byteCount; i++)
            {
                byte data = response[responseIndex++];

                for (int bit = 0; bit < 8; bit++)
                {
                    if (pointIndex >= quantity)
                        break;

                    ushort value = (ushort)((data >> bit) & 1);
                    ushort address = (ushort)(startAddress + pointIndex);

                    result.Add(
                        new Tuple<PointType, ushort>(
                            PointType.DIGITAL_OUTPUT,
                            address),
                        value);

                    pointIndex++;
                }
            }

            return result;
        }
		//mozda ispravnije
		/*public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
{
    Dictionary<Tuple<PointType, ushort>, ushort> resp = new Dictionary<Tuple<PointType, ushort>, ushort>();
    if((response[7] & 0x80) == 0)
    {
        int byteCount = response[8];
        int quantity = ((ModbusReadCommandParameters)CommandParameters).Quantity;
        ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
        for (int i = 0; i < byteCount && quantity > 0; i++)
        {
            var data = response[9 + i];
            for (int j = 0; j < 8 && quantity > 0; j++, quantity--)
            {
                var value = (ushort)(data & 1);
                resp.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, startAddress++), value);
                data >>= 1;
            }
        }
    }
    else
    {
        HandeException(response[8]);
    }
    return resp;
}*/
    }
}
