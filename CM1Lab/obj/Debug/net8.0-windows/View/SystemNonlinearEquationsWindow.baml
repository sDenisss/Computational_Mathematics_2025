   M S B A M L   `   `   `  ����A  =CM1Lab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:CM1Lab.ViewCM1Lab.View  5   	   0    *CM1Lab.View.SystemNonlinearEquationsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   5   	   'localclr-namespace:CM1Lab.View   5   	     � Title$$  SystemNonlinearEquationsWindow��5	   	   $
��1080��60   $
��1920��6>   .��5
      �  � RowDefinitions 5   
   �� 5      $��6* �6   �� 5      $��4* �6    � ColumnDefinitions 5   
   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��1* �6   .+�5   
     OxyPlot.Wpf.PlotView  $��0��6   $��0��6$   $��2��64     OxyPlot.Wpf.PlotViewBase   Model 6H   �� *	PlotModel+�� 5   
   $��1��6   $��Center=�6"   5      �� 5   
   $��1��6   $��0��6"   $
��Left=�62   $	��Top=�6M   $��10q�6e   .+�5)      �� -   5         ��Name$ systemComboBox��5)       �� SelectedValuePath$! SystemNonlinearEquationName��5*       �� DisplayMemberPath$! SystemNonlinearEquationName��5,      $
��20px��5-      $��100px��6%   $��5q�63   $��10(�6>    �� SelectedValue 5+      �� *SelectedSystem+ �� Mode$ TwoWay=�5-   >   �� 50      $��10(�6   Точность:61   �� 51      -   5        	 ��Name$	 Accuracy��51   0   $
��17px��6B   $��100px��6P   $��5q�6^   $��10(�6i   
���   ���6   �� *
Accuracy+6w   �� 52      $��10(�6   X0:6+   �� 53      -   5        $	 X0��53   *   $
��17px��66   $��100px��6D   $��5q�6R   $��10(�6]   
���   ���6   �� *X0+6k   �� 54      $��10(�6   Y0:6+   �� 55      -   5        $	 Y0��55   *   $
��17px��66   $��100px��6D   $��5q�6R   $��10(�6]   
���   ���6   �� *Y0+6k   5B      �� 5M   
   $��1��6   $��2��6"   $��Center=�62   .+�5N      �� -   5        $��Выбрать файл��5N      $	��100��6,   $��35��68   $
��10pxq�6D   
 �� 	IsDefault$

 True��6R   6c   �� 5O      -   5        $��Посчитать��5O      $	��100��6)   $��35��65   $
��10pxq�6A   $

 True��6O   6`   �� 5P      -   5        $��50��5P      $��50��6!   $��30(�6,   
�������$��20q�6M   $
��Left=�6Y   ���#222244$
��Bold"�5Q   *   .��6Q   <6W   5R      5S      5T      