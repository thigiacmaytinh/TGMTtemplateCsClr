#include "stdafx.h"

#include "TGMTbridgeSample.h"
#include "Example.h"
#include "TGMTbridge.h"

using namespace TGMT;
using namespace TGMTbridge;

////////////////////////////////////////////////////////////////////////////////////////////////////

Bitmap^ CbridgeSample::LoadImage(String^ filePath)
{
	std::string str = TGMT::TGMTbridge::SystemStr2stdStr(filePath);
	return TGMT::TGMTbridge::MatToBitmap(Example::LoadImage(str));
}