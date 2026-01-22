#pragma once

#include "slang.h"


__declspec(dllexport) const void* ISlangBlob_getBufferPointer(slang::IBlob** blob)
{
	return (*blob)->getBufferPointer();
}