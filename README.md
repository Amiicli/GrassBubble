# タイトル: Grassbubble

## スペック:
メモリー ：8GB以上  
グラフィック：AMD 5500XT/Nvidia 1660以上  
ストレージ:：200MB の空き容量  
プロセッサー ：2.3 GHz 8-Core Intel Core i9  
OS：AppleシリコンとintelCPUのMacOS、Windowsビルドがあります。  

このゲームは、Big Surを実行するMacOS Intel 64xで開発されました。  

## 解説:
バッタが慌てて跳ね回っています！魔法シャボンを使い、捕まえるのです！カメラを動かすにはマウスをドラッグすることができます。左マウスボタンを押すと魔法シャボンがマウスの指している位置で現れます！

すべてのGrasshopperコードは、ComputeShaderとメッシュのインスタンスを使用してGPU上で実行されます。アニメーションは頂点アニメーションテクスチャ（VATs）を使用して組み込まれました。ゲームをお楽しみいただければ幸いです！ 

## 興味深いファイル

  Assets/Scripts/LeveMain/GrassHopperControllerGPU.cs  
  Assets/Scripts/LeveMain/GrassHopper.compute  
  Assets/Editor/VATGenerator.cs  
  GrassBubble/Assets/Shaders/Grasshopper.shader  

# Title: Grassbubble

## Specifications:
RAM: 8 GB or more  
GPU: AMD 5500XT/Nvidia 1660 or better  
Storage: 200MB or more  
CPU: 2.3 GHz 8-Core Intel Core i9  
OS: There are builds for MacOS Intel or Universal, and Windows.   

This game was developed on MacOS Intel 64x running Big Sur.  

## Description:
There are grasshoppers running amok! In this game you can capture them with your magical Bubbles. Drag to move the camera around and click down to spawn a bubble.  

All Grasshopper code runs on the GPU using compute shaders and instanced meshes. Animations were incorporated with the use of vertex animation textures (VATs). I hope you enjoy the game!   

## Files of Interest

Assets/Scripts/LeveMain/GrassHopperControllerGPU.cs  
Assets/Scripts/LeveMain/GrassHopper.compute  
Assets/Editor/VATGenerator.cs  
GrassBubble/Assets/Shaders/Grasshopper.shader  
