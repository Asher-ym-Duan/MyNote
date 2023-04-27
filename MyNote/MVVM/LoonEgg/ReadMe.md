# LoongCore ��������MVVM
>��õ������˽⣬����ʵ��

[TOC]   

������ߣ, ��ͨ����Ԫ�������˽�MVVM��ܵ��ں�

## 12.Abstract�͵�Ԫ���Գ�����

1. ����ObservableObject��
�����������ǽ���ViewModel�Ļ��࣬��������û��ʵ�ʺ��壬Ӧ�ö�Ϊ�����࣬���˴���������
```c#
    public class ObservableObject : INotifyPropertyChanged
    {
              public event PropertyChangedEventHandler PropertyChanged;
 
    }
```
2. �½���Ԫ����
![01.Is Abstract](Figures/01.IsAbstract.png)

3. �ڵ�Ԫ������Ŀ��ȷ��ObservableObjectΪ������
```c#
    [TestClass]
    public class ObservableObject_Test
    {
        [TestMethod]
        public void IsAbstract() {

            var type = typeof(ObservableObject);
            Assert.IsTrue(type.IsAbstract);
        }
    }
```

4. �ڲ�����Դ�����������в���   
![01.Is Abstract Test](Figures/01.IsAbstractTest.png)

5. �޸�ObservableObject�����ͨ��
```c# 
    public abstract class ObservableObject : INotifyPropertyChanged
    {
              public event PropertyChangedEventHandler PropertyChanged;
 
    } 
```

## 13.PropertyChanged���Ըı�ʱ������ʲô

1. ```ObservableObject.cs```���Ըı�ʱ�����¼��ĵײ㷽��
```c#
        // TODO: 13-1 �������Ըı��¼��ķ���
        /// <summary>
        ///     �������Ըı��¼�
        /// </summary>
        ///     <param name="propertyName">�����ı�����Ե�����</param>
        /// <remarks>
        ///     ?. ������������˸�ViewModel���ˡ���Ƭ���Ż����������ⲿ���˶�����PropertyChanged
        ///     û����������ǿ��Եģ���������ܵ�Ӳ����дpropertyName
        /// </remarks>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke
                                (
                                    this,
                                    new PropertyChangedEventArgs(propertyName)
                                );
```
2. ```ObservableObject.cs```�������������������õ�ֵȷʵΪ��ֵʱ�����RaisePropertyChanged
```c#
        // TODO: 13-2 ����������
        /// <summary>
        /// �����µ�����ֵ,����ǡ�����¡�������<seealso cref="RaisePropertyChanged(string)"/>
        /// </summary>
        ///     <typeparam name="T">Ŀ�����Ե�����</typeparam>
        ///     <param name="target">Ŀ������</param>
        ///     <param name="value">�������µ�ֵ</param>
        ///     <param name="propertyName">[��Ҫ����]Ŀ�����Ե����ƣ��Զ��ƶ�</param>
        /// <returns>[true]Ŀ�������ѱ����£�</returns>
        protected bool SetProperty<T>
            (
                    ref T target, // Ŀ������
                    T value,      // ���¡�ֵ
                    [CallerMemberName] string propertyName = null
            ) {
            if (EqualityComparer<T>.Default.Equals(target, value))
                return false;

            target = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
```
3. ��Ԫ����
```c#
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// TODO: 13-3 ����Logger��¼��
using LoongEgg.LoongLogger;

namespace LoongEgg.LoongCore.Test
{
    [TestClass]
    public class ObservableObject_Test
    {
        // TODO: 13-4 ���Գ�ʼ��
        /// <summary>
        /// ��ʼ�����ԣ��������в��Է���ǰ����
        /// </summary>
        /// <remarks>
        ///     LoongEgg.LoongLogger���ҵ�һ����Դ��Ŀ������Բ�ʹ��
        /// </remarks>
        [TestInitialize]
        public void EnabledLogger() {
            LoggerManager.Enable(LoggerType.File, LoggerLevel.Debug);
        }
        
        /// <summary>
        /// ������ȷ��
        /// </summary>
        [TestMethod]
        public void IsAbstract() {
            var type = typeof(ObservableObject);
            Assert.IsTrue(type.IsAbstract);
        }

        // TODO: 13-5 ���һ��������
        /// <summary>
        /// <see cref="ObservableObject"/>��һ����������
        /// </summary>
        public class ObservableObjectSample : ObservableObject
        {
            /// <summary>
            /// ��������
            /// </summary>
            public int PropertySample {
                get => _PropertySample;
                set => SetProperty(ref _PropertySample, value);
            }
            /// <summary>
            /// �����ֶ�
            /// </summary>
            private int _PropertySample;

        }

        // TODO: 13-6 ���Ըı�ʱ�ᷢ��ʲô
        /// <summary>
        /// ���Ըı䣬�һ������¼�ȷ��
        /// </summary>
        [TestMethod] 
        public void CanPropertyChangedRaised() {
            bool isPropertyChangeRaised = false;// �¼��������

            // ��ʼ��һ���������
            ObservableObjectSample sample = new ObservableObjectSample();

            // ע�����Ըı�ʱ�Ĵ����¼�
            sample.PropertyChanged += (s, args) =>
                                        {
                                            isPropertyChangeRaised = true;
                                            LoggerManager.WriteDebug($"PropertyName:{args.PropertyName}");
                                        };

            // �ı�����
            sample.PropertySample = 666;
            Assert.IsTrue(isPropertyChangeRaised);
            Assert.AreEqual(sample.PropertySample, 666);
        }

        // TODO: 13-7 �������Ի���
        /// <summary>
        /// �����в�����ɺ���ã�ע��LoggerManager
        /// </summary>
        [TestCleanup]
        public void DisableLogger() {
            LoggerManager.WriteDebug("LoggerManager is clean up...");
            LoggerManager.Disable();
        }

    }
}

```

## 14.����ȷ��ʱ������֪ͨ

1. ���Բ������ı��ʱ��Ҫ�����¼�
```c#
        // TODO: 14-1 ������ֵ�����ڵ�ǰֵʱ������֪ͨ
        /// <summary>
        /// ������ֵ�����ڵ�ǰֵʱ������֪ͨ
        /// </summary>
        public void WhenPropertyEqualsOldValue_NotRaised() {
            bool isPropertyChangeRaised = false;// �¼��������

            // ��ʼ��һ���������
            // ע�����︳��һ����ʼֵ
            ObservableObjectSample sample = new ObservableObjectSample { PropertySample = 666};

            // ע�����Ըı�ʱ�Ĵ����¼�
            sample.PropertyChanged += (s, args) =>
                                        {
                                            isPropertyChangeRaised = true;
                                            LoggerManager.WriteDebug( 
                                                $"Event is raised by PropertyName={args.PropertyName}, value={sample.PropertySample}");
                                        };

            // �ı�����
            sample.PropertySample = 666;
            Assert.IsFalse(isPropertyChangeRaised); // ע�����������Flase
            Assert.AreEqual(sample.PropertySample, 666);
        }

```
2. ����ViewModelBase.cs
```c#
    // TODO: 14-2 ���ViewModel�Ļ���
    /// <summary>
    /// ViewModel�Ǽ̳��ڴ�
    /// </summary>
    public abstract class ViewModelBase : ObservableObject { }
```
3. ���������˵����Ըı��¼����ɱ�����������

![14.People](Figures/14.People.png)  
��```ViewModelBase_Test.cs```���Ӳ�����
```c#
public class People: ViewModelBase
        {

            public string FamilyName {
                get => _FamilyName;
                set {
                    if (SetProperty(ref _FamilyName, value))
                        RaisePropertyChanged("FullName");
                }
            }
            private string _FamilyName = "[NotDefined]";


            public string LastName {
                get => _LastName;
                set {
                    if (SetProperty(ref _LastName, value))
                        RaisePropertyChanged(nameof(FullName));
                }
            }
            private string _LastName = "[Unknown]";

            public string FullName => $"{FamilyName} - {LastName}";
        }
```
4.�����ĵ�Ԫ����
```c#
using System;
using LoongEgg.LoongLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoongEgg.LoongCore.Test
{
    [TestClass]
    public class ViewModelBase_Test
    {   /// <summary>
        /// ��ʼ�����ԣ��������в��Է���ǰ����
        /// </summary>
        /// <remarks>
        ///     LoongEgg.LoongLogger���ҵ�һ����Դ��Ŀ������Բ�ʹ��
        /// </remarks>
        [TestInitialize]
        public void EnabledLogger() {
            LoggerManager.Enable(LoggerType.File, LoggerLevel.Debug);
            LoggerManager.WriteDebug("Test initialized ok ....");
        }
        
        // TODO: 14-3��Ʋ�����People
        public class People: ViewModelBase
        {

            public string FamilyName {
                get => _FamilyName;
                set {
                    if (SetProperty(ref _FamilyName, value))
                        RaisePropertyChanged("FullName");
                }
            }
            private string _FamilyName = "[NotDefined]";


            public string LastName {
                get => _LastName;
                set {
                    if (SetProperty(ref _LastName, value))
                        RaisePropertyChanged(nameof(FullName));
                }
            }
            private string _LastName = "[Unknown]";

            public string FullName => $"{FamilyName} - {LastName}";
        }

        // TODO: 14-4 ������ǿ���������Ըı��¼�
        [TestMethod]
        public void CanRaisedByOtherProperty() {

            People people = new People();
            bool isRaised = false;
            people.PropertyChanged += (s, e) =>
                                        {
                                            isRaised = true;
                                            if(e.PropertyName == "FullName") {
                                                LoongLogger.LoggerManager.WriteDebug($"FullName is changed to -> {people.FullName}");
                                            }
                                        };

            people.FamilyName = "Alpha";
            people.LastName = "Jet";
            Assert.IsTrue(isRaised);
        }
         
        /// <summary>
        /// �����в�����ɺ���ã�ע��LoggerManager
        /// </summary>
        [TestCleanup]
        public void DisableLogger() {
            LoggerManager.WriteDebug("LoggerManager is clean up...");
            LoggerManager.Disable();
        }
    }
}

```

## 15.ICommand�����ʵ��
1. ICommand��ʵ��```DelegateCommand.cs```
```c#
using System;
using System.Windows.Input;

/* 
 | ����΢�ţ�InnerGeeker
 | ��ϵ���䣺LoongEgg@163.com 
 | ����ʱ�䣺2020/4/12 18:28:22
 | ��Ҫ��;��
 | ���ļ�¼��
 |			 ʱ��		�汾		����
 */
namespace LoongEgg.LoongCore
{

    public class DelegateCommand : ICommand
    {
        /*---------------------------------------- Fields ---------------------------------------*/
        /// <summary>
        /// �ɻ�ķ���
        /// </summary>
        private readonly Action<object> _Execute;
        /// <summary>
        /// �жϿ��Ըɻ�ķ���
        /// </summary>
        private readonly Predicate<object> _CanExecute;

        public bool CanExecuteCache { get; private set; } = true;
 
        /*------------------------------------- Constructors ------------------------------------*/
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="execute">�ɻ�ķ���</param>
        /// <param name="canExecute">�жϿ��Ըɻ�ķ���</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute) {
            _Execute = execute ?? throw new ArgumentNullException("execute ����Ϊ��");
            _CanExecute = canExecute;
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="execute">�ɻ�ķ���</param>
        public DelegateCommand(Action<object> execute) : this(execute, null) { }
         
        public event EventHandler CanExecuteChanged;

        /*------------------------------------ Public Methods -----------------------------------*/
        /// <summary>
        /// ����Ƿ����ִ������
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
           bool canExecute = _CanExecute?.Invoke(parameter) ?? true;

            if(canExecute != CanExecuteCache) {
                CanExecuteCache = canExecute;
                RaiseCanExecuteChanged();
            }

            return canExecute;
        }

        /// <summary>
        /// ִ���������
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => _Execute(parameter);

        /// <summary>
        /// ������ִ�иı��¼�
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

```
2.DelegateCommand�ĵ�Ԫ����
```c#
using System;
using LoongEgg.LoongLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoongEgg.LoongCore.Test
{
    [TestClass]
    public class DelegateCommand_Test
    {
        /// <summary>
        /// ��ʼ�����ԣ��������в��Է���ǰ����
        /// </summary>
        /// <remarks>
        ///     LoongEgg.LoongLogger���ҵ�һ����Դ��Ŀ������Բ�ʹ��
        /// </remarks>
        [TestInitialize]
        public void EnabledLogger() {
            LoggerManager.Enable(LoggerType.File, LoggerLevel.Debug);
            LoggerManager.WriteDebug("Test initialized ok ....");
        }

        /// <summary>
        /// ����ڹ�������execute����Ϊnull
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowExectptionIfActionParameterIsNull() {
            var command = new DelegateCommand(null);
        }

        /// <summary>
        /// Action���Ա�����ί��ִ��
        /// </summary>
        [TestMethod]
        public void ExecuteAction_CanInvokes() {
            bool invoked = false;

            void action(object obj) => invoked = true;

            var command = new DelegateCommand(action);
            command.Execute(null);

            Assert.IsTrue(invoked);
        }

        /// <summary>
        /// CanExecuteΪNullʱ����Ĭ�Ͽ���ִ��
        /// </summary>
        [TestMethod]
        public void CanExecute_IsTrueByDefault() {
            var command = new DelegateCommand(obj => { });
            Assert.IsTrue(command.CanExecute(null));
        }

        /// <summary>
        /// CanExecute�����ж������ִ��
        /// </summary>
        [TestMethod]
        public void CanExecute_FalsePredicate() {
            var command = new DelegateCommand
                                    (
                                        obj => { },
                                        obj => (int)obj == 0
                                    );
            Assert.IsFalse(command.CanExecute(6));
        }

        /// <summary>
        /// CanExecute�����ж��������ִ��
        /// </summary>
        [TestMethod]
        public void CanExecute_TruePredicate() {
            var command = new DelegateCommand
                                    (
                                        obj => { },
                                        obj => (int)obj == 6
                                    );
            Assert.IsTrue(command.CanExecute(6));
        }

        [TestMethod]
        public void CanExecuteChanged_Raised() {
            var command = new DelegateCommand
                                    (
                                        obj => { },
                                        obj => (int)obj == 6
                                    );
            bool isCanExecuteChanged = false;
            command.CanExecuteChanged += (s, e) =>
            {
                isCanExecuteChanged = true;
                LoggerManager.WriteDebug($"CanExecuteChanged Raised by {s.ToString()}");
            };
            Assert.IsTrue(command.CanExecute(6));
            Assert.IsFalse(command.CanExecute(66));
            Assert.IsTrue(isCanExecuteChanged);
        }

        /// <summary>
        /// �����в�����ɺ���ã�ע��LoggerManager
        /// </summary>
        [TestCleanup]
        public void DisableLogger() {
            LoggerManager.WriteDebug("LoggerManager is clean up...");
            LoggerManager.Disable();
        }
    }
}

```
## 16.�ҵ�MVVM��Ŀ�ṹ���ڿ���̨����WPF
### 1.�ҵ���Ŀ�ṹ
- ```AppConsole```����̨���򣬸�����װView��ViewModel 
- ```LoongEgg.LoongCore```��ͨ��⣬MVVM���Ŀ�ܣ��ṩ��ViewModel�Ļ���
- ```LoongEgg.LoongCore.Test```���Ŀ�ܵĵ�Ԫ������Ŀ
- ```LoongEgg.ViewModels```��ͨ��⣬ViewModel����������ƣ�С��ĿҲ������ҵ���߼�
- ```LoongEgg.ViewModels.Test```ViewModel�ǵĵ�Ԫ����
- ```LoongEgg.Views```�Զ���ؼ��⣬View�����⼯���  
![16.Project Layout](Figures/16.ProjectLayout.png)

### 2.�ڿ���̨����WPF����
- ��Ҫ������  
![16.Reference](Figures/16.Reference.png)
- Program.cs
```c#
using LoongEgg.ViewModels;
using LoongEgg.Views;
using System;
using System.Windows;

namespace AppConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            //CalculatorViewModel viewModel = new CalculatorViewModel { Left = 111, Right = 222, Answer = 333 };
            CalculatorView view = new CalculatorView { DataContext = viewModel };
            Application app = new Application();
            app.Run(view);
        }
    }
}

```
## 17.û��MVVM
- ```MainWindow.xaml```ǰ̨����
```xaml
<Window
    x:Class="NoMVVM.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:NoMVVM"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    Title="MainWindow"
    Width="800"
    Height="450"
    FontSize="32"
    mc:Ignorable="d">
    <Grid Margin="5">
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition Width="AUTO" />
            <ColumnDefinition />
            <ColumnDefinition Width="AUTO" />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>

        <!--  ��������  -->
        <TextBox
            x:Name="left"
            Grid.Column="0"
            VerticalAlignment="Center"
            Text="666" />

        <!--  �������  -->
        <StackPanel
            Grid.Column="1"
            VerticalAlignment="Center"
            ButtonBase.Click="Button_Click">
            <Button
                Width="80"
                Height="80"
                Margin="5"
                Content="+" />
            <Button
                Width="80"
                Height="80"
                Margin="5"
                Content="-" />
            <Button
                Width="80"
                Height="80"
                Margin="5"
                Content="*" />
            <Button
                Width="80"
                Height="80"
                Margin="5"
                Content="/" />
        </StackPanel>

        <!--  �Ҳ������  -->
        <TextBox
            x:Name="right"
            Grid.Column="2"
            VerticalAlignment="Center"
            Text="999" />

        <!--  =��  -->
        <Label
            Grid.Column="3"
            VerticalAlignment="Center"
            Content="=" />

        <TextBlock
            x:Name="answer"
            Grid.Column="4"
            VerticalAlignment="Center"
            Text="Answer" />
    </Grid>
</Window>

```
- ```MainWindow.xaml.cs```��˴���  
```c#
using System.Windows;
using System.Windows.Controls;

namespace NoMVVM
{
    /// <summary>
    /// MainWindow.xaml �Ľ����߼�
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            if(e.Source is Button btn) {
                bool isDouble = false;  

                isDouble =  double.TryParse( left.Text, out double leftOpr);
                if (!isDouble) return;

                isDouble =  double.TryParse( right.Text, out double rightOpr);
                if (!isDouble) return;

                string opr = btn.Content.ToString();

                switch (opr) {
                    case "+":answer.Text = (leftOpr + rightOpr).ToString(); break;
                    case "-":answer.Text = (leftOpr - rightOpr).ToString(); break;
                    case "*":answer.Text = (leftOpr * rightOpr).ToString(); break;
                    case "/":answer.Text = (leftOpr / rightOpr).ToString(); break; 
                    default:
                        break;
                }
            }
               
        }
    }
}

```

## 18.��һ��ViewModel���׼�����
1. ��������ViewModel
```c#
using LoongEgg.LoongCore;
using System.Windows.Controls;
using System.Windows.Input;

namespace LoongEgg.ViewModels
{
    // TODO: 18-1 ��������ViewModel
    /// <summary>
    /// ��������ViewModel
    /// </summary>
    public class CalculatorViewModel: ViewModelBase
    {
        /*------------------------------------- Properties --------------------------------------*/        /// <summary>
        /// ��������
        /// </summary>
        public int Left {
            get => _Left;
            set => SetProperty(ref _Left, value);
        }
        protected int _Left;
         
        /// <summary>
        /// �Ҳ������
        /// </summary>
        public int Right {
            get => _Right;
            set => SetProperty(ref _Right, value);
        }
        protected int _Right;
         
        /// <summary>
        /// ������
        /// </summary>
        public int Answer {
            get => _Answer;
            set => SetProperty(ref _Answer, value);
        }
        protected int _Answer;

        /// <summary>
        /// ��������
        /// </summary>
        public ICommand OperationCommand { get; protected set; }

        /*------------------------------------- Constructor -------------------------------------*/
        /// <summary>
        /// Ĭ�Ϲ�����
        /// </summary>
        public CalculatorViewModel() {
            OperationCommand = new DelegateCommand(Operation);
        }

        /*----------------------------------- Private Methods -----------------------------------*/ 
        /// <summary>
        /// ����ľ���ִ�з���
        /// </summary>
        /// <param name="opr"></param>
        protected void Operation(object opr) {
            var self = opr as Button;
            switch (opr.ToString()) {
                case "+": Answer = Left + Right; break;
                case "-": Answer = Left - Right; break;
                case "*": Answer = Left * Right; break;
                case "/": Answer = Left / Right; break;
            };
        }
 
    }
}

```
2. CalculatorViewModel�ĵ�Ԫ����
```c#
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoongEgg.LoongCore.Test
{
    // TODO: 15-2 DelegateCommand�ĵ�Ԫ����
    [TestClass]
    public class DelegateCommand_Test
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowExeceptionIfExecuteParameterIsNULL() {
            var command = new DelegateCommand(null);
        }

        [TestMethod]
        public void Execute_CanInvokes() {
            bool invoked = false;

            var command = new DelegateCommand(
                                                   obj => { invoked = true; }
                                             );
            command.Execute(null);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void CanExecute_IsTrueByDefault() {
            var command = new DelegateCommand(obj => { });

           Assert.IsTrue(  command.CanExecute(null));
        }

        [TestMethod]
        public void CanExecute_TruePredicate() {
            var command = new DelegateCommand
                (
                    obj => { },
                    obj =>  (int)obj == 666
                );
            Assert.IsTrue(command.CanExecute(666));
        }

        [TestMethod]
        public void CanExecute_FalsePredicate() {
            var command = new DelegateCommand
                (
                    obj => { },
                    obj =>  (int)obj == 666
                );
            Assert.IsFalse(command.CanExecute(66));
        }
    }
}

```
## 19.ViewModel��View�Ļ�ʦ��Bindingǰ����ؿ������Ƶ
https://www.bilibili.com/video/BV1ci4y1t7D6/

### KeyPoint
- �ڿ���̨����WPF������DataContextΪ���ViewModel
- DesignModel�̳�ViewModel����Ϊ������Ϊ�Լ��ľ�̬���ԣ��������ʱ�󶨣���С��������
- ����İ󶨲�Ҫ���ǰ�CommandParameter(�����Ҫ)

### 1.��ʼ��ViewModel��ע��View(����ע��)
```c#
using LoongEgg.ViewModels;
using LoongEgg.Views;
using System;
using System.Windows;

namespace AppConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {

            // TODO: 19-1 ��ʼ��ViewModel��ע��View
            // ��ʼ��һ��ViewModel������һЩ��ʼֵ��ʾ��DesignModel��һ��
            CalculatorViewModel viewModel = new CalculatorViewModel { Left = 111, Right = 222, Answer = 333 };

            // ��ViewModel��ֵ��View��DataContext
            CalculatorView view = new CalculatorView { DataContext = viewModel };

            Application app = new Application();
            app.Run(view);
        }
    }
}
```

### 2.����һ��DesignModel�Է������ʱ��
```c#
using LoongEgg.ViewModels;

namespace LoongEgg.Views
{
    /*
	| 
	| WeChat: InnerGeek
	| LoongEgg@163.com 
	|
	*/
    // TODO: 19-2 ����һ��DesignModel�Է������ʱ��
    public class CalculatorDesignModel: CalculatorViewModel
    {
        public static CalculatorDesignModel Instance => _Instance ?? (_Instance = new CalculatorDesignModel());
        private static CalculatorDesignModel _Instance;

        public CalculatorDesignModel() : base() {
            Left = 999;
            Right = 666;
            Answer = 233;
        }
    }
}
```

### 3.���View��ViewModel��Xaml�еİ�
**��Ҫ�����������ʱDataContext**```d:DataContext="{x:Static local:CalculatorDesignModel.Instance}"```
**Binding��ᷢ�֣�ŶӴ�����﷨��ʾ��**
```xml
<Window
    x:Class="LoongEgg.Views.CalculatorView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:LoongEgg.Views"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:vm="clr-namespace:LoongEgg.ViewModels;assembly=LoongEgg.ViewModels"
    Title="Calculator View - 1st MVVM Application"
    Width="800"
    Height="450"
    d:DataContext="{x:Static local:CalculatorDesignModel.Instance}"
    FontSize="52"
    WindowStartupLocation="CenterScreen"
    mc:Ignorable="d">
    <Window.Resources>
        <Style TargetType="{x:Type Button}">
            <Setter Property="Width" Value="80" />
            <Setter Property="Height" Value="80" />
            <Setter Property="Margin" Value="5" />
        </Style>

        <Style TargetType="{x:Type TextBox}">
            <Setter Property="HorizontalAlignment" Value="Stretch" />
            <Setter Property="VerticalAlignment" Value="Center" />
        </Style>
    </Window.Resources>
    <Grid Margin="5">
        <Grid.ColumnDefinitions>
            <ColumnDefinition />
            <ColumnDefinition Width="auto" />
            <ColumnDefinition />
            <ColumnDefinition Width="auto" />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>

        <!--  ���Ĳ�����  -->
        <TextBox Grid.Column="0" Text="{Binding Left}" />

        <!--  �������  -->
        <StackPanel Grid.Column="1" VerticalAlignment="Center">
            <Button
                Command="{Binding OperationCommand}"
                CommandParameter="+"
                Content="+" />
            <Button
                Command="{Binding OperationCommand}"
                CommandParameter="-"
                Content="-" />
            <Button
                Command="{Binding OperationCommand}"
                CommandParameter="*"
                Content="*" />
            <Button
                Command="{Binding OperationCommand}"
                CommandParameter="/"
                Content="/" />
        </StackPanel>

        <!--  �Ҳ������  -->
        <TextBox Grid.Column="2" Text="{Binding Right}" />

        <Label
            Grid.Column="3"
            VerticalAlignment="Center"
            Content="=" />

        <!--  ������  -->
        <TextBox Grid.Column="4" Text="{Binding Answer}" />
    </Grid>
</Window>

```
## 20.ȫ����򵥵�IValueConverterʵ��
### 1.һ�㷽����IValueConverter��
```c#
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

/* 
 | ����΢�ţ�InnerGeeker
 | ��ϵ���䣺LoongEgg@163.com 
 | ����ʱ�䣺2020/4/14 19:51:26
 | ��Ҫ��;��
 | ���ļ�¼��
 |			 ʱ��		�汾		����
 */
namespace LoongEgg.Views
{
    /// <summary>
    /// ����ת<see cref="Brush"/>
    /// </summary>
    public class IntToBrushConverter : IValueConverter
    {         
        /*------------------------------------ Public Methods -----------------------------------*/
        
        /// <summary>
        /// ����ת<see cref="Brush"/><see cref="IValueConverter.Convert(object, Type, object, CultureInfo)"/>
        /// </summary> 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                return null;
            }else if((int) value < 18) {
                return Brushes.Green;
            }else {
                return Brushes.Blue;
            }
        }

        /// <summary>
        /// ����Ҫ
        /// </summary> 
        public object ConvertBack
            (
                object value, 
                Type targetType, 
                object parameter, 
                CultureInfo culture
            ) => throw new NotImplementedException(); 
    }
}

```
### 2.һ�㷽����IValueConverterʹ��
- ����Ϊ��̬��Դ
```xml
<Window.Resources>
        <local:IntToBrushConverter x:Key="intToBrushConverter" />
</Window.Resources>
``` 
- ������Ҫ�ĵط���̬����
```xml
 <!--  ���Ĳ�����  -->
        <TextBox
            Grid.Column="0"
            Foreground="{Binding Left, Converter={StaticResource intToBrushConverter}}"
            Text="{Binding Left}" />

```
### 3.һ�����ݵ�IValueConverter����
```c#
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

/* 
 | ����΢�ţ�InnerGeeker
 | ��ϵ���䣺LoongEgg@163.com 
 | ����ʱ�䣺2020/4/14 20:07:32
 | ��Ҫ��;��
 | ���ļ�¼��
 |			 ʱ��		�汾		����
 */
namespace LoongEgg.Views
{
    /// <summary>
    /// ֵת�����Ļ��࣬��һ�����ͷ�����������Ҫʵ�ֵ�ֵת������
    /// </summary>
    /// <typeparam name="T">��Ҫ��ֵת��������������</typeparam>
    public abstract class BaseValueConverter <T>
        : MarkupExtension, IValueConverter
        where T: class, new()
    {
        /*---------------------------------------- Fields ---------------------------------------*/
        /// <summary>
        /// ֵת������ʵ��
        /// </summary>
        private static T _Instance; 
          
        /*------------------------------------ Public Methods -----------------------------------*/
        /// <summary>
        /// Ϊ����Xaml��ֱ��ʹ��<see cref="IValueConverter"/>����ʵ�ֵ�һ������<see cref="MarkupExtension.ProvideValue(IServiceProvider)"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>����ֵת�����ĵ�ʵ��</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
            => _Instance ?? (_Instance = new T());
            

        /// <summary>
        /// <see cref="IValueConverter.Convert(object, Type, object, CultureInfo)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
            

        /// <summary>
        /// ��ǰ̨UI�е�ֵת������̨ViewModelһ���ò���
        /// </summary>
        /// <param name="value">UI�е�ֵ</param>
        /// <param name="targetType">Ŀ������</param>
        /// <param name="parameter">�����ת������</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)  
            => throw new NotImplementedException();

       
    }
}
```
### 4.ValueConverter�����ʵ��
```c#
using System;
using System.Globalization;
using System.Windows.Media;

/* 
 | ����΢�ţ�InnerGeeker
 | ��ϵ���䣺LoongEgg@163.com 
 | ����ʱ�䣺2020/4/14 20:18:51
 | ��Ҫ��;��
 | ���ļ�¼��
 |			 ʱ��		�汾		����
 */
namespace LoongEgg.Views
{
    /// <summary>
    /// ��򵥵�ֵת����ʵ��
    /// </summary>
    public class AdvanceIntToBrushConverter : BaseValueConverter<AdvanceIntToBrushConverter>
    { 
        /*------------------------------------ Public Methods -----------------------------------*/
         
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                return null;
            }else if( (int) value < 18) {
                return Brushes.Green;
            }else {
                return Brushes.Yellow;
            }
        }
    }
}

```
### 5.��򵥵�ValueConverterʹ��
```xml
 <!--  �Ҳ������  -->
        <TextBox
            Grid.Column="2"
            Background="{Binding Right, Converter={local:AdvanceIntToBrushConverter}}"
            Text="{Binding Right}" />
```
