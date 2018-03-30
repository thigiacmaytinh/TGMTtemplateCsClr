#include "Example.h"

cv::Mat Example::LoadImage(std::string filePath)
{
	cv::Mat mat = cv::imread(filePath);

	cv::putText(mat, "thigiacmaytinh.com", cv::Point(0, 35), 1, cv::FONT_HERSHEY_DUPLEX, cv::Scalar(0,0, 255), 2);
	return mat;
}