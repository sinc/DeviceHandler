using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    public interface IPin<DataType>
    {
        /// <summary>
        /// Родительский апплет
        /// </summary>
        IAppletHandler<DataType> AppletHandler { get; }

        /// <summary>
        /// Пин подключен
        /// </summary>
        bool IsConnect { get; }

        /// <summary>
        /// Присоединить к пину
        /// </summary>
        /// <param name="ConnectedPin">Присоединяемый пин</param>
        /// <returns></returns>
        void Connect(IPin<DataType> ConnectedPin);

        /// <summary>
        /// Отсоединить пин
        /// </summary>
        /// <param name="Pin"></param>
        /// <returns></returns>
        void Disconnect(IPin<DataType> Pin);

        /// <summary>
        /// Отсоединить все пины
        /// </summary>
        /// <returns></returns>
        void Disconnect();

        /// <summary>
        /// Базовый поток пина
        /// </summary>
        AppletStream<DataType> BaseStream { get; }
    }
}
