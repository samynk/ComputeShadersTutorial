#pragma once
#include <vector>
#include <string>
#include <fstream>
#include <sstream>
#include <iostream>
#include <regex>
#include <glm/vec2.hpp>
#include <glm/vec3.hpp>
#include <algorithm>

class FaceVertex {
public:
    uint32_t ti{ 0 };
    uint32_t iti{ 0 };
    uint32_t fvi{ 0 };
    uint32_t vi{ 0 };
    uint32_t tci{ 0 };
    uint32_t ni{ 0 };

    // Comparison operator for sorting and comparison
    bool operator<(const FaceVertex& f2) const {
        return std::tie(vi, tci, ni) < std::tie(f2.vi, f2.tci, f2.ni);
    }

    // Equality operator
    bool operator==(const FaceVertex& f2) const {
        return vi == f2.vi && tci == f2.tci && ni == f2.ni;
    }
};


class ObjReader {


public:
    ObjReader(const std::string& filePath)
        : location(filePath), lineNumber(0) {}

    void parseObjFile();
    void buildOutputMesh();

    // Getters for the output data
    std::vector<uint32_t> getOIndexBuffer() const {
        return oIndexBuffer;
    }

    uint32_t getNumVertices() const {
        return oVertices.size();
    }

    const std::vector<glm::vec3>& getOVertices() const {
        return oVertices;
    }

    const std::vector<glm::vec2>& getOTexcoords() const {
        return oTexcoords;
    }

    const std::vector<glm::vec3>& getONormals() const {
        return oNormals;
    }

private:
    void readLine(const std::string& line, int lineNumber);

    void readVertex(const std::string& line, int lineNumber);

    void readTexcoord(const std::string& line, int lineNumber);

    void readNormal(const std::string& line, int lineNumber);

    void readFace(const std::string& line, int lineNumber);

    bool readFaceVertex(FaceVertex& face, const std::string& component);

    std::string location;
    int lineNumber;

    // Not normalized
    std::vector<glm::vec3> vertices;
    std::vector<glm::vec2> texcoords;
    std::vector<glm::vec3> normals;
    std::vector<FaceVertex> faceVertices;

    // Normalized buffers
    std::vector<uint32_t> oIndexBuffer;
    std::vector<glm::vec3> oVertices;
    std::vector<glm::vec2> oTexcoords;
    std::vector<glm::vec3> oNormals;

    // Regular expressions for parsing
    std::regex faceRegex{ R"((\d*)/(\d*)/(\d*))" };
    std::regex whitespaceRegex{ R"(\s+)" };
};