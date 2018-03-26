#include "stdafx.h"

#include "TGMTbridgeSample.h"
#include "Example.h"
#include "TGMTbridge.h"

using namespace TGMTbridge;


////////////////////////////////////////////////////////////////////////////////////////////////////

bool CbridgeSample::DetectStar(String^ filePath)
{
	std::string path = TGMT::TGMTbridge::SystemStr2stdStr(filePath);
	return Example::GetInstance()->DetectStar(path);
}

////////////////////////////////////////////////////////////////////////////////////////////////////

Bitmap^ CbridgeSample::GetImage()
{
	cv::Mat mat = Example::GetInstance()->GetImage();
	return TGMT::TGMTbridge::MatToBitmap(mat);
}

////////////////////////////////////////////////////////////////////////////////////////////////////

int CbridgeSample::GetTotalStar()
{
	return Example::GetInstance()->GetTotalStar();
}

////////////////////////////////////////////////////////////////////////////////////////////////////

int CbridgeSample::GetTotalContour()
{
	return Example::GetInstance()->GetTotalContour();
}