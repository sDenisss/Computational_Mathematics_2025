   M S B A M L   `   `   `  ����A  =CM1Lab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:CM1Lab.ViewCM1Lab.View  5   	   )    #CM1Lab.View.ApproximationFuncWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   5   	   'localclr-namespace:CM1Lab.View   5   	     � Title$  ApproximationFuncWindow��5	   	   $
��1080��6)   $
��1920��67   .��5
      �  � RowDefinitions 5   
   �� 5      $��6* �6   �� 5      $��4* �6    � ColumnDefinitions 5   
   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��3* �6   �� 5      $��1* �6   .+�5   
     OxyPlot.Wpf.PlotView  $��0��6   $��0��6$   $��2��64     OxyPlot.Wpf.PlotViewBase   Model 6H   �� *	PlotModel+�� 5   
   $��1��6   $��Center=�6"   5      �� 5   
   $��1��6   $��0��6"   $
��Left=�62   $	��Top=�6M   $��10q�6e   .+�5'      �� $��10(�6   Размерность:64   �� 5(      -   5         ��Name$ Accuracy��5(   ,   $
��17px��6>   $��100px��6L   $��5q�6Z   $��10(�6e   
���   ���6   �� *Size+6s   5)      �� 5+   
   $��1��6   $��1��6"   $
��Left=�62   .+�5,      �� $��13(�6   $
��Left=�6&   ;9Коэфы апроксимирующих функций:6a   �� 5-      $��13(�6   $
��Left=�6&   ��6A   �� *ApproximationResultsString+6o   �� 5/      $��13(�6   $
��Left=�6&   31Среднеквадрат. отклонение:6]   �� 50      $��13(�6   $
��Left=�6&   ��6A   �� *SredneKvOtklon+6c   �� 52      $��13(�6   $
��Left=�6&   Эпсилон:6K   �� 53      $��13(�6   $
��Left=�6&   ��6A   �� *Eps+6X   �� 55      $��13(�6   $
��Left=�6&    Коэф корреляции:6S   �� 56      $��13(�6   $
��Left=�6&   ��6A   �� *PearsonCorrelation+6g   �� 58      $��13(�6   $
��Left=�6&   $"Коэф Детерминации:6U   �� 59      $��13(�6   $
��Left=�6&   ��6A   �� *CoefDetermination+6f   �� 5;      $��13(�6   $
��Left=�6&   CAНаилучшая апроксимирующая функция:6e   �� 5<      $��13(�6   $
��Left=�6&   ��6A   �� *BestApproximation+6f   5>      �� 5B   
   $��0��6   $��2��6"   $
��Left=�62   $	��Top=�6M   $��1q�6e   .+�5D      �� $��10(�6   DBКоэффициенты(в 1-ой строке x, во 2-ой y):6Q   � 5F      -   5         �Name$ CoefficientGrid��5F      5H      5I      �� 5L   
   $��1��6   $��2��6"   $��Center=�62   .+�5M      �� -   5        $��Выбрать файл��5M      $	��100��6,   $��35��68   $
��10pxq�6D    �� 	IsDefault$
 True��6R   6c   �� 5N      -   5        $��Подтведить��5N      $	��100��6*   $��35��66   $
��10pxq�6B   $
 True��6P   6a   �� 5O      -   5        $��Посчитать��5O      $	��100��6)   $��35��65   $
��10pxq�6A   $
 True��6O   6`   �� 5P      -   5        $��50��5P      $��50��6!   $��30(�6,   
�������$��20q�6M   $
��Left=�6Y   ���#222244$
��Bold"�5Q      .��6F   <6L   5S      5T      5U      