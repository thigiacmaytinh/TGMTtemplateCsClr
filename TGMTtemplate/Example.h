#pragma once
#include "stdafx.h"

class Example
{
	cv::Mat m_mat;
	int mtotal;
public:
	Example::Example();
	Example::~Example();

	bool DetectStar(cv::Mat matInput);
	bool DetectStar(std::string filePath);
};

