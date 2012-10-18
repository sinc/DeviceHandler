using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FTD2XX_NET;
using System.Windows.Forms;

namespace DeviceHandler
{
    public class CHDSample
    {
        public const int kTrueADCBits = 24;
        public const float kADCMask = (float)(1 << kTrueADCBits);
        public const float kReferenceVoltage = 312500.0f;
        public const double kDistance = 0.06;   //mm

        public int takt { get; private set; }
        public double[,] voltages { get; private set; }
        public double[] hall { get; private set; }
        public double[] U0 { get; private set; }
        public double x { get; private set; }
        public double y { get; private set; }
        public bool isValid { get; private set; }
        public CHDSample previousSample { get; private set; }

        public double dx
        {
            get
            {
                return Math.Abs(x - ((previousSample != null) ? previousSample.x : 0));
            }
        }

        public double dy
        {
            get
            {
                return Math.Abs(y - ((previousSample != null) ? previousSample.y : 0));
            }
        }
        /*
        public CHDSample(double[] h, double x)
        {
            hall = h;
            this.x = x;
        }
        */
        public CHDSample(byte[] rawBytes, CHDSample prevSample, double[] K, double[] Un)
        {
            previousSample = prevSample;
            if (prevSample == null)
            {
                takt = 0;
                voltages = new double[CHDDriver.kCHDChannels, 4];
                isValid = false;
            }
            else
            {
                takt = (prevSample.takt == 3) ? 0 : prevSample.takt + 1;
                voltages = prevSample.voltages.Clone() as double[,];
                isValid = prevSample.isValid || takt == 3;
            }
            hall = new double[CHDDriver.kCHDChannels];
            U0 = new double[CHDDriver.kCHDChannels];
            if (rawBytes != null)
            {
                for (int ch = 0; ch < CHDDriver.kCHDChannels; ch++)
                {
                    voltages[ch, takt] = ((float)ToBigEndian.Convert(BitConverter.ToInt32(rawBytes, ch * 4)) / kADCMask - 1.0f) * kReferenceVoltage;
                    hall[ch] = ((voltages[ch, 0] - voltages[ch, 1] - voltages[ch, 2] + voltages[ch, 3]) / 4.0 - Un[ch]) / K[ch];
                    U0[ch] = (voltages[ch, 0] - voltages[ch, 1] + voltages[ch, 2] - voltages[ch, 3]) / 4.0;
                }
                x = ToBigEndian.Convert(BitConverter.ToInt32(rawBytes, CHDDriver.kCHDChannels * 4)) * kDistance;
                //y = ToBigEndian.Convert(BitConverter.ToInt32(rawBytes, CHDDriver.kCHDChannels * 4 + 4));
            }
            else
            {
                isValid = false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}; {3}", hall[0], hall[1], hall[2], x);
        }
    }

    public class CHDDriver: Applet
    {
        private const double B0 = 50;
        private const int kAverage = 20;
        public const int kCHDChannels = 3;
        private FTDI m_CHDDevice;
        private OutPin<CHDSample> m_outputPin;
        private Thread m_measureThread;
        private volatile bool m_isMeasure = false;

        public CHDDriver(GraphBuilder gb, FTDI device, string deviceDescriptor, uint baudRate): base(gb)
        {
            FTDI.FT_STATUS fs;
            if (device != null)
            {
                m_CHDDevice = device;
                if ((fs = device.OpenBySerialNumber(deviceDescriptor)) != FTDI.FT_STATUS.FT_OK)
                    throw new Exception(string.Format("Невозможно открыть устройство. Устройство не готово. ({0})", fs));
                if ((fs = device.SetBaudRate(baudRate)) != FTDI.FT_STATUS.FT_OK)
                    throw new Exception(string.Format("Устройство не поддерживает заданную скорость ({0})", fs));
                m_outputPin = RegisterOutputPin<CHDSample>();

                K = Enumerable.Range(0, kCHDChannels).Select(i => 1.0).ToArray();
                Un = Enumerable.Range(0, kCHDChannels).Select(i => 0.0).ToArray();
                alpha = new double[kCHDChannels];
            }
        }

        private bool waitBytes(int count)
        {
            uint enableBytes = 0;
            System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();
            watcher.Start();
            while (enableBytes < count)
            {
                if (watcher.ElapsedMilliseconds < 5000)
                    m_CHDDevice.GetRxBytesAvailable(ref enableBytes);
                else
                    return false;
            }
            return true;
        }

        private byte[] sendCommand(byte command, params byte[] args)
        {
            FTDI.FT_STATUS fs;
            byte[] paket = { command/*, args??*/ } ;
            uint writtenBytes = 0;
            
            if ((fs = m_CHDDevice.Write(paket, paket.Length, ref writtenBytes)) != FTDI.FT_STATUS.FT_OK)
                throw new Exception(string.Format("Ошибка записи: {0}", fs));
            if (writtenBytes != paket.Length)
                throw new Exception("Неверная длина пакета");
            while (true)
            {
                if (waitBytes(1))
                {
                    //первый байт это метка команды, ответ на которую
                    //пересылается
                    if ((fs = m_CHDDevice.Read(paket, 1, ref writtenBytes)) != FTDI.FT_STATUS.FT_OK)
                        throw new Exception(string.Format("GetCom. Ошибка чтения: {0}", fs));
                    if (paket[0] != (byte)command)
                        continue;
                    //wait packet header
                    if (waitBytes(1))
                    {
                        if ((fs = m_CHDDevice.Read(paket, 1, ref writtenBytes)) != FTDI.FT_STATUS.FT_OK)
                            throw new Exception(string.Format("GetAnswer. Ошибка чтения: {0}", fs));
                        if ((paket[0] & 0x03) == 0)
                        {
                            //число байт = число каналов + байт контрольной суммы + байт на заряд батареи
                            uint bytesToRead = (uint)(paket[0] & 0xFC) + 2 + 8;/*automatic multiply by 4*/
                            paket = new byte[bytesToRead];
                            if (waitBytes((int)bytesToRead))
                            {
                                if ((fs = m_CHDDevice.Read(paket, bytesToRead, ref writtenBytes)) != FTDI.FT_STATUS.FT_OK)
                                    throw new Exception(string.Format("GetData. Ошибка чтения: {0}", fs));
                                //контрольная сумма вычисляется по данным и по заряду батареи
                                if (paket[0] == Crc8.GetCrc8(paket, bytesToRead - 1, 1))
                                {
                                    byte[] data = new byte[paket.Length - 1];
                                    Array.Copy(paket, 1, data, 0, data.Length);
                                    return data;
                                }
                            }
                            else
                                break;
                        }
                        else
                        {
                            //throw new Exception(string.Format("Ошибка #{0}", (CHG_ErrorMsg)(paket[0] & 2)));
                            break;
                        }
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return null;
        }

        public void startMeasure()
        {
            if (!m_isMeasure)
            {
                m_measureThread = new Thread(new ThreadStart(
                    delegate()
                        {
                            m_isMeasure = true;
                            CHDSample lastSample = null;
                            byte command = 0x13;
                            while (m_isMeasure)
                            {
                                lastSample = new CHDSample(sendCommand(command), lastSample, K, Un);
                                //send new sample
                                if (lastSample.isValid)
                                    m_outputPin.Push(lastSample);
                                if (--command < 0x10)
                                    command = 0x13;
                            }
                        }));
                m_measureThread.IsBackground = true;
                m_measureThread.Start();
            }
        }

        public void stopMeasure()
        {
            if (m_isMeasure && m_measureThread != null)
            {
                m_isMeasure = false;
                m_measureThread.Join(100);
                m_measureThread = null;
            }
        }

        /// <summary>
        /// Возвращает массив усредененных значений холловского напряжения
        /// </summary>
        /// <param name="count">Число усреднений</param>
        /// <returns></returns>
        private double[] averMeasure(int count)
        {
            double[] aver = new double[kCHDChannels];
            CHDSample lastSample = null;
            byte command = 0x13;

            while (count > 0)
            {
                lastSample = new CHDSample(sendCommand(command), lastSample, K, Un);
                if (lastSample.isValid)
                {
                    for (int i = 0; i < kCHDChannels; i++)
                    {
                        aver[i] += lastSample.hall[i];
                    }
                    count--;
                }
                if (--command < 0x10)
                    command = 0x13;
            }
            for (int i = 0; i < kCHDChannels; i++)
                aver[i] /= kAverage;
            return aver;
        }

        /// <summary>
        /// Остаточное напряжение небаланса
        /// </summary>
        public double[] Un { get; private set; }

        /// <summary>
        /// Крутизна датчиков
        /// </summary>
        public double[] K { get; private set; }

        /// <summary>
        /// Углы поворота датчиков
        /// </summary>
        public double[] alpha { get; private set; }

        public void calibrate()
        {
            stopMeasure();
            if (MessageBox.Show("Установите датчик в положение 0 градусов", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                double[] U0 = averMeasure(kAverage);
                if (MessageBox.Show("Установите датчик в положение 90 градусов", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    double[] U1 = averMeasure(kAverage);
                    if (MessageBox.Show("Установите датчик в положение 180 градусов", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        double[] U2 = averMeasure(kAverage);
                        for (int i = 0; i < kCHDChannels; i++)
                        {
                            Un[i] = (U0[i] + U2[i]) / 2;
                            U0[i] -= Un[i];
                            U1[i] -= Un[i];
                            U2[i] -= Un[i];
                        }
                        for (int i = 0; i < kCHDChannels; i++)
                        {
                            double u02 = U0[i] * U2[i];
                            double u12 = U1[i] * U2[i];
                            double u01 = U0[i] * U1[i];
                            double braket = 1.0 / U2[i] - 1.0 / U0[i] + 2.0 / (U2[i] - U0[i]);

                            K[i] = Math.Sqrt(u02 * u02 + u12 * u12 / 4 - u02 * U1[i] * U1[i] / 2 + u01 * u01 / 4) * braket / 3;
                            
                            U0[i] /= K[i];
                            U1[i] /= K[i];
                            U2[i] /= K[i];
                        }
                        alpha[0] = 0;
                        alpha[1] = Math.Acos((U0[0] * U0[1] + U2[0] * U2[1] - 2 * U1[0] * U1[1]) / 2);
                        alpha[2] = Math.Acos((U0[0] * U0[2] + U2[0] * U2[2] - 2 * U1[0] * U1[2]) / 2) + Math.PI;                        
                    }
                }
            }
        }
    }
}
