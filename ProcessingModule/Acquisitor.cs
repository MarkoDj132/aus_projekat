using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for periodic polling.
    /// </summary>
    public class Acquisitor : IDisposable
	{
		private AutoResetEvent acquisitionTrigger;
        private IProcessingManager processingManager;
        private Thread acquisitionWorker;
		private IStateUpdater stateUpdater;
		private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Acquisitor"/> class.
        /// </summary>
        /// <param name="acquisitionTrigger">The acquisition trigger.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="stateUpdater">The state updater.</param>
        /// <param name="configuration">The configuration.</param>
		public Acquisitor(AutoResetEvent acquisitionTrigger, IProcessingManager processingManager, IStateUpdater stateUpdater, IConfiguration configuration)
		{
			this.stateUpdater = stateUpdater;
			this.acquisitionTrigger = acquisitionTrigger;
			this.processingManager = processingManager;
			this.configuration = configuration;
			this.InitializeAcquisitionThread();
			this.StartAcquisitionThread();
		}

		#region Private Methods

        /// <summary>
        /// Initializes the acquisition thread.
        /// </summary>
		private void InitializeAcquisitionThread()
		{
			this.acquisitionWorker = new Thread(Acquisition_DoWork);
			this.acquisitionWorker.Name = "Acquisition thread";
		}

        /// <summary>
        /// Starts the acquisition thread.
        /// </summary>
		private void StartAcquisitionThread()
		{
			acquisitionWorker.Start();
		}

        /// <summary>
        /// Acquisitor thread logic.
        /// </summary>
        private void Acquisition_DoWork()
        {
            //TO DO: IMPLEMENT
            int brojac = 0;
            IEnumerable<IConfigItem> configItems = configuration.GetConfigurationItems();

            while (true)
            {
                acquisitionTrigger.WaitOne();

                stateUpdater.LogMessage("Acquisition tick " + brojac);
                foreach (IConfigItem configItem in configItems)
                {
                    configItem.SecondsPassedSinceLastPoll += 1;

                    if (configItem.SecondsPassedSinceLastPoll == configItem.AcquisitionInterval)
                    {
                        processingManager.ExecuteReadCommand(
                            configItem,
                            configuration.GetTransactionId(),
                            configuration.UnitAddress,
                            configItem.StartAddress,
                            configItem.NumberOfRegisters
                            );
                        configItem.SecondsPassedSinceLastPoll = 0;
                    }
                }
            }
        }
		/*private void Acquisition_DoWork()
        {
            //stateUpdater.LogMessage("ACQ THREAD ALIVE");
            //TO DO: IMPLEMENT
            int brojac = 0;
            ushort transactionId = 0;
            //stateUpdater.UpdateConnectionState(ConnectionState.CONNECTED);
            while (true)
            {
                acquisitionTrigger.WaitOne();
                //stateUpdater.LogMessage("ACQ TRIGGER FIRED");
                brojac++;
                //stateUpdater.LogMessage("Acquisition tick " + brojac);

                if (brojac > 1000000)
                    brojac = 0;

                try
                {
                    //stateUpdater.UpdateConnectionState(ConnectionState.CONNECTED);
                    foreach (var item in configuration.GetConfigurationItems())
                    {
                        
                        // digital every 2nd cycle
                        if ((item.RegistryType == PointType.DIGITAL_OUTPUT ||
                             item.RegistryType == PointType.DIGITAL_INPUT) &&
                            brojac % 2 == 0)
                        {
                            processingManager.ExecuteReadCommand(
                                item,
                                transactionId++,
                                configuration.UnitAddress,
                                item.StartAddress,
                                item.NumberOfRegisters);
                            stateUpdater.LogMessage("READ SENT: " + item.RegistryType);
                        }
                        
                        // analog every 3rd cycle
                        if ((item.RegistryType == PointType.ANALOG_INPUT ||
                             item.RegistryType == PointType.ANALOG_OUTPUT) &&
                            brojac % 3 == 0)
                        {
                            processingManager.ExecuteReadCommand(
                                item,
                                transactionId++,
                                configuration.UnitAddress,
                                item.StartAddress,
                                item.NumberOfRegisters);
                            stateUpdater.LogMessage("READ SENT: " + item.RegistryType);
                        }
                    }
                }
                
                catch (Exception ex)
                {
                    stateUpdater.LogMessage(ex.ToString());
                    stateUpdater.LogMessage("READ ERROR: " + ex.Message);
                    stateUpdater.UpdateConnectionState(ConnectionState.DISCONNECTED);
                    
                }
            }
        }*/


        #endregion Private Methods

            /// <inheritdoc />
        public void Dispose()
		{
			acquisitionWorker.Abort();
        }
	}
}