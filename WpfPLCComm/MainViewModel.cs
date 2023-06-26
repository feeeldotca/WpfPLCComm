using Modbus.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfPLCComm
{
    //MVVM design model
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        Modbus.Device.ModbusIpMaster master;
        private ushort _value;
        
        // provide value source for reading temprature and data binding to show the value
        public ushort Value
        {
            get { return _value; }
            set { 
                _value = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }
        // for write RPM button
        public ICommand BtnCommand { get; set; }

        public MainViewModel()
        {
            TcpClient tcpClient = new TcpClient();

            // connect to ModbusSlave app
            tcpClient.Connect("127.0.0.1", 502);
            master = Modbus.Device.ModbusIpMaster.CreateIp(tcpClient);

            // Logic for Reading Holding Register 
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(500);
                    //read Holding register from modbusslave device 1, start address 0, read 1 coil status
                    ushort[] value = master.ReadHoldingRegisters(1, 0, 1);
                    // for WPF value binding
                    Value= value[0];
                }
            });

            // Logic for Write to register
            BtnCommand = new CommandBase()
            {
                //DoExecute = new Action<object>(DoBtnCommand)
                DoExecute = DoBtnCommand
            };            
        }

        private ushort  _input;
        // provide write value source for RPM and data binding
        public ushort  Input
        {
            get { return _input; }
            set { _input = value; }
        }

        private void DoBtnCommand(object obj)
            {
            master.WriteSingleRegister(1, Input);
            }
    }

   
}
