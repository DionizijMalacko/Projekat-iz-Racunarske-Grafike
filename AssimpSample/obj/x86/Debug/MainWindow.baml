   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    AssimpSample   AssimpSample.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	   -   5          � Title$  SharpGL WPF Application��5   	   $	��650��6)   $
��1025��66   .��5      �  � RowDefinitions 5   
   �� 5      $	��30* �6   $��30��6)   5
      �� 5      $
��289* �6   5      .+�5   
      SharpGL.WPF.OpenGLControl  -   5        $��openGLControl��5      $��1��6H      DrawFPS .    RenderContextType$	 FBO=�5         	FrameRate$ 60Q�64   c� 5   
   -   5         c�Name$ toolBar��5      $��Stretch=�6#   $��0��6B   $	��Top=�6O   $��30��6h   $��30��6t   .@�5      �� $��Duzina piste��6   $��25��6+   $	��Top=�67   �� 5      -   5         ��Name$ airstrip��5      $��25��6'   $	��Top=�63   $	��100��6K    � Minimum$
 3000Q�6|    � Maximum$
 4000Q�6�   	 � Value$
	 4000Q�6�   
 � LargeChange$	
 0.1Q�6�   �� 5      $��Skaliranje aviona��6   $��25��60   $	��Top=�6<   �� 5      -   5        $ scale��5      $��25��6$   $	��Top=�60   $	��100��6H   $
 0.25Q�6v   $ 2Q�6�   $	 1Q�6�   $	
 0.1Q�6�   �� 5      $��Brzina animacije��6   $��25��60   $	��Top=�6<   �� 5      -   5        $ 	animation��5      $��25��6(   $	��Top=�64   $	��100��6L   $	 0.5Q�6~   $	 2.5Q�6�   $	 1Q�6�   $	
 0.1Q�6�   �� 5      $��15��6    �� Orientation$ 
Horizontal=�6%   .+�5       �� -   5         ��Name$ c1��5        d� 	IsChecked$
 TrueY�6D   .��6r   Stacionarno svetlo6�   �� 5!      -   5        $ c2��5!       d� IsThreeState .  $
 TrueY�6\   .��6�   	Reflektor6�   5"      5#      5%      5&      