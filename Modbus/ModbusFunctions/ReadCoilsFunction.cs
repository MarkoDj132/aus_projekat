using Common;
using Modbus.FunctionParameters;
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
    }
}
