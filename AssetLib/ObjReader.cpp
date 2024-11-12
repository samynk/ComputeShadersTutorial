#include "ObjReader.h"

void ObjReader::parseObjFile() {
	std::ifstream file(location);
	if (!file.is_open()) {
		std::cerr << "Error opening file: " << location << std::endl;
		return;
	}

	std::string line;
	while (std::getline(file, line)) {
		readLine(line, ++lineNumber);
	}
}

void ObjReader::readLine(const std::string& line, int lineNumber) {
	if (line.starts_with("vt")) {
		readTexcoord(line, lineNumber);
	}
	else if (line.starts_with("vn")) {
		readNormal(line, lineNumber);
	}
	else if (line.starts_with("v")) {
		readVertex(line, lineNumber);
	}
	else if (line.starts_with("f")) {
		readFace(line, lineNumber);
	}
}

void ObjReader::readVertex(const std::string& line, int lineNumber) {
	std::istringstream s(line);
	std::string token;
	s >> token; // Skip 'v'

	float x, y, z;
	if (s >> x >> y >> z) {
		vertices.emplace_back(x, y, z);
	}
	else {
		std::cerr << "Wrong number of components at line " << lineNumber << std::endl;
	}
}

void ObjReader::readTexcoord(const std::string& line, int lineNumber) {
	std::istringstream s(line);
	std::string token;
	s >> token; // Skip 'vt'

	float u, v;
	if (s >> u >> v) {
		texcoords.emplace_back(u, v);
	}
	else {
		std::cerr << "Wrong number of components at line " << lineNumber << std::endl;
	}
}

void ObjReader::readNormal(const std::string& line, int lineNumber) {
	std::istringstream s(line);
	std::string token;
	s >> token; // Skip 'vn'

	float x, y, z;
	if (s >> x >> y >> z) {
		normals.emplace_back(x, y, z);
	}
	else {
		std::cerr << "Wrong number of components at line " << lineNumber << std::endl;
	}
}

void ObjReader::readFace(const std::string& line, int lineNumber) {
	std::istringstream s(line);
	std::string token;
	s >> token; // Skip 'f'

	std::vector<std::string> components;
	std::string part;
	while (s >> part) {
		components.push_back(part);
	}

	if (components.size() < 3) {
		std::cerr << "Face at line " << lineNumber << " does not have enough vertices." << std::endl;
		return;
	}

	FaceVertex fv1, fv2, fv3;
	int triangleIndex = faceVertices.size() / 3;
	fv1.ti = triangleIndex;
	fv1.iti = 2;
	fv2.ti = triangleIndex;
	fv2.iti = 1;
	fv3.ti = triangleIndex;
	fv3.iti = 0;

	bool fv1Ok = readFaceVertex(fv1, components[0]);
	bool fv2Ok = readFaceVertex(fv2, components[1]);
	bool fv3Ok = readFaceVertex(fv3, components[2]);

	if (fv1Ok && fv2Ok && fv3Ok) {
		faceVertices.push_back(fv1);
		faceVertices.push_back(fv2);
		faceVertices.push_back(fv3);
	}
}

bool ObjReader::readFaceVertex(FaceVertex& face, const std::string& component) {
	if (component.find('/') == std::string::npos) {
		// Only vertex index
		face.vi = std::stoi(component);
		return true;
	}
	else {
		// Vertex/Texture/Normal
		std::smatch matches;
		if (std::regex_match(component, matches, faceRegex)) {
			for (size_t i = 1; i < matches.size(); ++i) {
				const std::ssub_match& match = matches[i];
				if (match.length() > 0) {
					int number = std::stoi(match.str());
					switch (i) {
					case 1:
						face.vi = number;
						break;
					case 2:
						face.tci = number;
						break;
					case 3:
						face.ni = number;
						break;
					}
				}
			}
			return true;
		}
		else {
			std::cerr << "Invalid face format at line " << lineNumber << ": " << component << std::endl;
			return false;
		}
	}
}

void ObjReader::buildOutputMesh() {
	// Sort faceVertices by vi, tci, and ni
	std::sort(faceVertices.begin(), faceVertices.end());

	uint32_t currentVertexIndex = 0;
	uint32_t fvi = std::numeric_limits<uint32_t>::max(); // current face vertex index
	uint32_t fti = std::numeric_limits<uint32_t>::max(); // current texcoord index
	uint32_t fni = std::numeric_limits<uint32_t>::max(); // current normal index

	oIndexBuffer.resize(faceVertices.size(), std::numeric_limits<uint32_t>::max());

	for (const auto& face : faceVertices) {
		if (face.vi != fvi || face.tci != fti || face.ni != fni) {
			fvi = face.vi;
			fti = face.tci;
			fni = face.ni;

			if (fvi > 0) {
				oVertices.push_back(vertices[fvi - 1]);
			}
			if (fti > 0) {
				oTexcoords.push_back(texcoords[fti - 1]);
			}
			if (fni > 0) {
				oNormals.push_back(normals[fni - 1]);
			}

			++currentVertexIndex;
		}
		int fIndex = face.ti * 3 + face.iti;
		oIndexBuffer[fIndex] = currentVertexIndex - 1;
	}
}