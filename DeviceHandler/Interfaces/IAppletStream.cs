﻿using System;
using System.Collections.Generic;

namespace DeviceHandler
{
<<<<<<< HEAD
    interface IAppletStream<DataType>
=======
    public delegate void NewDataEnabled();

    public interface IAppletStream<DataType>
>>>>>>> efc741a98f33971ae4d5dc05caeb29a8ee67b9c5
    {
        /// <summary>
        /// Длина потока
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Записать в поток один отсчет
        /// </summary>
        /// <param name="sample"></param>
        void Write(DataType sample);

        /// <summary>
        /// Считать из потока в буфер. Возвращает число считанных отсчетов
        /// </summary>
        /// <param name="buf">буфер</param>
        /// <param name="pos">позиция в потоке</param>
        /// <param name="index">индекс смещения в буфере</param>
        /// <param name="count">количество элементов</param>
        /// <param name="TimeOut">время ожидания считывания</param>
        /// <returns></returns>
        int Read(DataType[] buf, int pos, int index, int count, int TimeOut);
    }
}
