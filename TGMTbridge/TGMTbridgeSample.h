#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;

namespace TGMTbridge
{

	public ref class CbridgeSample
	{
		
	public:
		static bool DetectStar(String^ filePath);
		static Bitmap^ GetImage();
		static int GetTotalStar();
		static int GetTotalContour();
	};
}
