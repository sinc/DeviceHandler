using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    interface IAppletStreamReader<DataType>
    {
        /// <summary>
        /// Позиция в потоке
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Поток для чтения
        /// </summary>
        IAppletStream<DataType> Stream { get; }

        /// <summary>
        /// Считать из потока следующий отсчет
        /// </summary>
        /// <returns></returns>
        DataType Read();

        /// <summary>
        /// Считать в буфер. Возвращает число считанных отсчетов.
        /// </summary>
        /// <param name="buf">Буфер</param>
        /// <param name="index">Смещение в буфере</param>
        /// <param name="Count">Число считываемых отсчетов</param>
        /// <returns></returns>
        int Read(DataType[] buf, int index, int Count);

        /// <summary>
        /// Закрыть чтение из потока
        /// </summary>
        void Close();
    }
}
