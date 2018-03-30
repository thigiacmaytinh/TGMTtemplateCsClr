#include "Example.h"

Example* Example::instance = nullptr;

////////////////////////////////////////////////////////////////////////////////////////////////////

Example::Example()
{
}

////////////////////////////////////////////////////////////////////////////////////////////////////

Example::~Example()
{
}

////////////////////////////////////////////////////////////////////////////////////////////////////

bool Example::DetectStar(std::string filePath)
{
	m_totalStar = 0;
	m_totalContour = 0;

	cv::Mat im = cv::imread(filePath, CV_LOAD_IMAGE_COLOR);

	if (!im.data)
		return false;

	cv::Mat imgrey = im.clone();
	cv::cvtColor(im, imgrey, CV_RGB2GRAY);
	std::vector<std::vector<cv::Point> > contours;
	std::vector<cv::Vec4i> hierarchy;

	double divMaxSize = 0.175, divMinSize = 0.125;

#ifdef _DEBUG
	//cv::namedWindow("Image", CV_WINDOW_NORMAL | CV_WINDOW_KEEPRATIO | CV_GUI_EXPANDED);
#endif

	cv::threshold(imgrey, imgrey, 200, 255, 0);

	cv::findContours(imgrey, contours, hierarchy, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, cv::Point(0, 0));

	m_totalContour = contours.size();
	for (int i = 0; i < m_totalContour; i++)
	{
		
		std::cout << "sqrt(Area)/arcLength = " << sqrt(cv::contourArea(contours[i])) / cv::arcLength(contours[i], true) << std::endl;
		if (sqrt(cv::contourArea(contours[i])) / cv::arcLength(contours[i], true) < divMaxSize && sqrt(cv::contourArea(contours[i])) / arcLength(contours[i], true) > divMinSize)
		{
			cv::drawContours(im, contours, i, RED, 1, 8, hierarchy, 0, cv::Point());
			m_totalStar++;
		}
#ifdef _DEBUG
		//cv::imshow("Image", im);
#endif
	}
	m_mat = im;

	return true;
}

////////////////////////////////////////////////////////////////////////////////////////////////////

cv::Mat Example::GetImage()
{
	return m_mat;
}

////////////////////////////////////////////////////////////////////////////////////////////////////

int Example::GetTotalStar()
{
	return m_totalStar;
}

////////////////////////////////////////////////////////////////////////////////////////////////////

int Example::GetTotalContour()
{
	return m_totalContour;
}