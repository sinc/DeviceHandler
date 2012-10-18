using System;
using System.Collections.Generic;
using System.Linq;

namespace DeviceHandler
{
    interface IAppletStreamReader<DataType>
    {
        /// <summary>
        /// открыт ли поток для чтения
        /// </summary>
        bool isOpened { get; }

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
        /// <param name="shift">На сколько сместить позицию в потоке</param>
        /// <param name="count">Число считываемых отсчетов</param>
        /// <returns></returns>
        int Read(DataType[] buf, int index, int shift, int count);

        /// <summary>
        /// Закрыть чтение из потока
        /// </summary>
        void Close();
    }
}
