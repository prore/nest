   M S B A M L   `   `   `  ���� E  APhotoPaint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:PhotoPaint
PhotoPaint      PhotoPaint.Window1   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     e aMicrosoft.Surface.Presentation, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e351s)http://schemas.microsoft.com/surface/2008  !ppclr-namespace:PhotoPaint    � Title$  
PhotoPaint�� � 	Resources    EditButtonImage � RowDefinitions � ColumnDefinitions F�Name �� Stretch2  ,Microsoft.Surface.Presentation.SurfaceColors   ButtonForegroundBrushKey  EraseButtonImage��%#  &	        &	 :    � $��38��$��38��$��Center=�
������  �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� $ Pencil1��$
 Fill=�#���  t���p��X ����i0@�� ��@4 \�� єK� ݟ Ѡ�\C Q��X ����i0@�
C& 0�� � Z �j� �<>+ �N� ��J �e� �
C& 0�� $��0q�$��1��$��1��� 
������ $��Center=�$��38��$��38�� �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� $
 Path��$
 Fill=�#���  ����� <�BѶ�  �(�� TB��-� X���d� �`�� |ۈ�Ԥ�(�m �Ԥ�<8� ш�� @~�S��� �=yX�u@@�>� ��Aώ@@2n} �/��.r ����ȡ���o0�����6?�KV�� <�B�$$� ��$6�����^F�@��(/\�^ ��[��fQ �$U� p�dӼ�� �Lodţ L`p�� �gш�+ ,Iј$$� $��0q�$��1��$��1��.��� .+�9  3Microsoft.Surface.Presentation.Controls.ScatterView  .@�=  7Microsoft.Surface.Presentation.Controls.ScatterViewItem  $	��317��$	��423��$	��293��$	��846��7   0LibraryControlManipulationAreaBackgroundBrushKey#���  .���  �� $��7 ��� �� $��52 � �� $��7 ��� �� $��44 ��� $��44 ��� $��44 ��� �� $��7 �.+�(� $��1��$��5��$��1��$��1�� (� StretchDirection$
 Both=�	 (� Stretch$
	 Fill=�.��>  8Microsoft.Surface.Presentation.Controls.SurfaceInkCanvas  -   
  Name$
 DrawingPadCanvas��$	��263��$	��414��1   *LibraryControlScrollAreaBackgroundBrushKey   
Background# �  ;  5Microsoft.Surface.Presentation.Controls.SurfaceButton  -   $��EditModeButton��$��2��$��2��$��0q�$��3.5,7,3.5,7q�.��� -    �Name$ DrawButtonImage��$��38��$��38��$��Center=�
������  �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� -   $ Pencil2��$
 Fill=�#���  t���p��X ����i0@�� ��@4 \�� єK� ݟ Ѡ�\C Q��X ����i0@�
C& 0�� � Z �j� �<>+ �N� ��J �e� �
C& 0�� $��0q�$��1��$��1��  -   $��2��$��4��$��0q�$��3.5,7,3.5,7q�.��� -   $
 Undo��$��38��$��38��
������ $��Center=� �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� -   $ Undo1��$��20��$
 Fill=�#���  �����h nM �zY6 6>1 �^�U � h� f��� `  ��&� P� P�x�f �02� Ӕ*� |�� TK� �n��:0@�� 8[�� �B� � 0� ��� /� �� h�� 8�� Ӭ� <�o \�� ��2 t�� �\& �j�� x z]d $, �kL �PG Ѳ�x d�s ѷh ��s ѷh nM $��Center=�$��0q�$��Center=�$��1��$��1��A� -   $��CurrentColor��$��Stretch=�$��Stretch=�$��2��$��3��$��3.5,7,3.5,7q�
��� ����� -	   $��
ColorWheel��$��Resources\ColorWheel.png�� J� RenderTransformOrigin$ 0.5, 0.5&�$��2��$��3��$y�Hidden=�z���  �� ScaleX$ 4Q� �� ScaleY$ 4Q�  -
     Name$ PhotoPadSVI��$	��320��$	��427��$	��296��.���  �� $��7 ��� �� $��52 � �� $��44 ��� �� $��44 ��� $��44 ��� �� $��44 �.+�� $��0��$��7��$��0��$��5��.+��� -   $��Photo��(� $
 Both=�$
	 Fill=�.��  -   $
 PostcardCanvas�� J� IsHitTestVisible .  $	��320��$	��426��
 ����    UsesTouchShape .    -   $��2��$��2��$��0q�$��3.5,7,3.5,7q�.��� -   $ X��$��38��$��38��
������ $��Center=� �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� -   $ X2��$
 Fill=�#���  �����xu z	 ��	 ;u �^T� �s �x��#�l0@su �43�	 �x� "T� Q�2�&1�l0@���镲l0@l2�hT� x� ��	 �2Q�u ��镲l0@��s "T� �xu z	 $��0q�$��1��$��1��  -   $��2��$��3��$��0q�$��3.5,7,3.5,7q�.��� -   $ MoveButtonImage��$��38��$��38��
������ $��Center=� �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� -   $ Resize1��$
 Fill=�#���  ����p,T� ʚ�& ��� � ��- �l�� ��- �l�� �z ���� �z ���� ��[ р�@T� ����  � ���� X�� �l�� 0�� �l�� $�� � � $�� �T� ��X�[ $�� ��z $�� ��z 0�� Ѷ�- X�� Ѭ�-  � �����  ��T� ���- X�[ ���- �z ��z �z ��z p�- �l�[ p�- Q,T� ʚ�& ��$��Center=�$��Center=�$��1��$��1��  $	��320��$	��426��$	��296��$	��853��#���  .���  �� $��29 ��� �� $��52 � �� $��44 ��� �� $��44 ��� $��44 ��� $��44 ��� �� $��44 �.+�� $��0��$��7��$��1��$��1��.+�r� -    r�Name$ Movie�� r� LoadedBehavior$ Manual=� r� UnloadedBehavior$ Manual=�(� $
 Both=�$
	 Fill=�.��  -   $
 MovieCanvas��$	��240��$	��427��
 ����  .   .    -     Name$ RecordButton��$��2��$��2��$��0q�$��3.5,7,3.5,7q�$��Center=�.��� -   $ AnotateVideoImage��$��38��$��38��$��Center=�
������  �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� -   $ Pencil��$
 Fill=�#���  t���p��X ����i0@�� ��@4 \�� єK� ݟ Ѡ�\C Q��X ����i0@�
C& 0�� � Z �j� �<>+ �N� ��J �e� �
C& 0�� $��0q�$��1��$��1��  -   $ PauseButton��$��2��$��3��$��0q�$��3.5,7,3.5,7q�$��Center=�.��� -   $ Pause��$��38��$��38��
������  �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�� $��Center=�$��0q�$��Center=�$��1��$��1��.+��� -    ��Name$ Rectangle_Copy2��$
 Fill=�#���  $��9,0,0,0q�$��19��$
��Left=�$��5���� -   $ Rectangle_Copy3��$��5��$
 Fill=�#���  $
��Left=�$��0q�$��19��  -   $ 
PlayButton��$��2��$��4��$��0q�$��3.5,7,3.5,7q�$��Center=�.��� -   $
 Play��
������ $��38��$��38�� �� $��0.25* ��� �� $��0.25* � �� $��0.25* ��� �� $��0.25* �.+�F� -   $ Play1��#���  $
 Fill=�$��0q�$��Center=�$��19��"����        �    ���@T� @T� 	$��1��$��Center=�$��10��$��1��