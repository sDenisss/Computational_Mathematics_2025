   M S B A M L   `   `   `  ����A  =CM1Lab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:CM1Lab.ViewCM1Lab.View  5   	   *    $CM1Lab.View.NonlinearEquationsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:CM1Lab.View     5   	   R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   5   	     � Title$  NonlinearEquationsWindow��5	   	   $
��1080��6*   $
��1920��68   .��5
      �  � RowDefinitions 5   
   �� 5      $��6* �6   �� 5      $��4* �6    � ColumnDefinitions 5   
   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��1* �6   .+�5   
     OxyPlot.Wpf.PlotView  $��0��6   $��0��6$   $��2��64     OxyPlot.Wpf.PlotViewBase   Model 6H   �� *	PlotModel+�� 5   
   $��1��6   $��Center=�6"   5      �� 5   
   $��1��6   $��0��6"   $
��Left=�62   $	��Top=�6M   $��10q�6e   .+�5"      �� -   5         ��Name$ methodsComboBox��5"       �� SelectedValuePath$ 
MethodName��5#       �� DisplayMemberPath$ 
MethodName��5%      $
��20px��5&      $��100px��6%   $��5q�63   $��10(�6>    �� SelectedValue 5$      �� *SelectedMethod+ �� Mode$ TwoWay=�5&   >   �� 5(      -   5        $ functionsComboBox��5(      $ FunctionName��5)      $ FunctionName��5+      $
��20px��5,      $��100px��6%   $��5q�63   $��10(�6>    5*      �� *SelectedFunction+$ TwoWay=�5,   >   �� 5.      $��10(�6   Точность:61   �� 5/      -   5        	 ��Name$	 Accuracy��5/   0   $
��17px��6B   $��100px��6P   $��5q�6^   $��10(�6i   
���   ���6   �� *
Accuracy+6w   �� 50      $��10(�6   Интервал a:63   �� 51      -   5        $	 	Ainterval��51   1   $
��17px��6D   $��100px��6R   $��5q�6`   $��10(�6k   
���   ���6   �� *	Ainterval+6y   �� 52      $��10(�6   Интервал b:63   �� 53      -   5        $	 	Binterval��53   1   $
��17px��6D   $��100px��6R   $��5q�6`   $��10(�6k   
���   ���6   �� *	Binterval+6y   �� 54      $��10(�6   ,*Начальное приближение:6>   �� 55      -   5        $	 X0��55   *   $
��17px��66   $��100px��6D   $��5q�6R   $��10(�6]   
���   ���6   �� *X0+6k   5A      �� 5C   
   $��1��6   $��1��6"   $
��Left=�62   .+�5D      �� $��10(�6   $
��Left=�6&   BisectionMethod:6S   �� 5E      $��10(�6   $
��Left=�6&   ��6A   �� *BisectionRoot+6b   �� 5M      $��10(�6   $
��Left=�6&   SecantMethod:6P   �� 5N      $��10(�6   $
��Left=�6&   ��6A   �� *
SecantRoot+6_   �� 5P      $��10(�6   $
��Left=�6&   SimpleIterationMethod:6Y   �� 5Q      $��10(�6   $
��Left=�6&   ��6A   �� *SimpleIterationRoot+6h   5S      �� 5\   
   $��0��6   $��2��6"   $
��Left=�62   $	��Top=�6M   $��1q�6e   .+�5^      �� $��10(�6   Уравнение:62   �� 5_      -   5        $	 EquationFormula��5`      $
��25px��5a      $��600px��6    $��5q�6.   $��10(�69   
���   ���5_      �� *EquationFormula+
 �� UpdateSourceTrigger$
 PropertyChanged=�5a   G   5b      �� 5f   
   $��1��6   $��2��6"   $��Center=�62   .+�5g      �� -   5        $��Выбрать файл��5g      $	��100��6,   $��35��68   $
��10pxq�6D    �� 	IsDefault$
 True��6R   6c   �� 5h      -	   5        $��Посчитать��5h      $	��100��6)   $��35��65   $
��10pxq�6A   $
 True��6O   6`   �� 5i      -
   5        $��50��5i      $��50��6!   $��30(�6,   
�������$��20q�6M   $
��Left=�6Y   ���#222244$
��Bold"�5j   &   .��6M   <6S   5l      5m      5n      