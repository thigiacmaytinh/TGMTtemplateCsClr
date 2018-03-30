#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;

namespace TGMTbridge
{

	public ref class CbridgeSample
	{
		
	public:
		static Bitmap^ LoadImage(String^ filePath);
	};
}
