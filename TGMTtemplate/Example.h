#pragma once
#include "stdafx.h"

class Example
{
	static Example* instance;
	cv::Mat m_mat;
	int m_totalStar;
	int m_totalContour;

	

public:
	static Example* GetInstance()
	{
		if (!instance)
			instance = new Example();
		return instance;
	}

	Example::Example();
	Example::~Example();

	bool DetectStar(cv::Mat matInput);
	bool DetectStar(std::string filePath);

	cv::Mat GetImage();
	int GetTotalStar();
	int GetTotalContour();
};

