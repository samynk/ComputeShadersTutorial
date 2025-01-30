namespace AssetLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text.RegularExpressions;


    /// <summary>
    /// C# equivalent of the FaceVertex structure.
    /// </summary>
    public class FaceVertex : IComparable<FaceVertex>, IEquatable<FaceVertex>
    {
        public uint ti { get; set; }    // Triangle index
        public uint iti { get; set; }   // In-triangle index
        public uint fvi { get; set; }   // Face-vertex index (not heavily used here)
        public uint vi { get; set; }    // Vertex index
        public uint tci { get; set; }   // Texcoord index
        public uint ni { get; set; }    // Normal index

        // Compare by (vi, tci, ni) to replicate operator<
        public int CompareTo(FaceVertex other)
        {
            if (other == null) return 1;

            int cmpVi = vi.CompareTo(other.vi);
            if (cmpVi != 0) return cmpVi;

            int cmpTci = tci.CompareTo(other.tci);
            if (cmpTci != 0) return cmpTci;

            return ni.CompareTo(other.ni);
        }

        // Equality check replicates operator==
        public bool Equals(FaceVertex other)
        {
            if (other == null) return false;
            return (vi == other.vi && tci == other.tci && ni == other.ni);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FaceVertex);
        }

        public override int GetHashCode()
        {
            // Combine vi, tci, ni for a simple unique hash
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (int)vi;
                hash = hash * 31 + (int)tci;
                hash = hash * 31 + (int)ni;
                return hash;
            }
        }
    }

    /// <summary>
    /// C# conversion of the ObjReader class.
    /// Reads a .obj file and populates data in arrays for vertices, normals, texcoords, etc.
    /// </summary>
    public class ObjReader
    {
        private readonly string location;
        private int lineNumber;

        // Not normalized
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector2> texcoords = new List<Vector2>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<FaceVertex> faceVertices = new List<FaceVertex>();

        // Normalized buffers
        private List<uint> oIndexBuffer = new List<uint>();
        private List<Vector3> oVertices = new List<Vector3>();
        private List<Vector2> oTexcoords = new List<Vector2>();
        private List<Vector3> oNormals = new List<Vector3>();

        // Regular expressions for parsing
        private readonly Regex faceRegex = new Regex(@"(\d*)/(\d*)/(\d*)");
        private readonly Regex whitespaceRegex = new Regex(@"\s+");

        /// <summary>
        /// Constructor that sets the path to the obj file.
        /// </summary>
        /// <param name="filePath"></param>
        public ObjReader(string filePath)
        {
            location = filePath;
            lineNumber = 0;
        }

        /// <summary>
        /// Reads the file line by line and parses each line.
        /// </summary>
        public void ParseObjFile()
        {
            if (!File.Exists(location))
            {
                Console.Error.WriteLine($"Error opening file: {location}");
                return;
            }

            using (var file = new StreamReader(location))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    lineNumber++;
                    ReadLine(line, lineNumber);
                }
            }
        }

        /// <summary>
        /// Processes a single line, deciding what type of data it contains.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        private void ReadLine(string line, int lineNumber)
        {
            if (line.StartsWith("vt"))
            {
                ReadTexcoord(line, lineNumber);
            }
            else if (line.StartsWith("vn"))
            {
                ReadNormal(line, lineNumber);
            }
            else if (line.StartsWith("v"))
            {
                ReadVertex(line, lineNumber);
            }
            else if (line.StartsWith("f"))
            {
                ReadFace(line, lineNumber);
            }
        }

        /// <summary>
        /// Parses a vertex line, e.g. "v x y z".
        /// </summary>
        private void ReadVertex(string line, int lineNumber)
        {
            // split by whitespace, ignoring empty entries
            var tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            // tokens[0] is "v", so we need tokens[1..3] for x, y, z
            if (tokens.Length < 4)
            {
                Console.Error.WriteLine($"Wrong number of components at line {lineNumber}");
                return;
            }

            if (float.TryParse(tokens[1], out float x) &&
                float.TryParse(tokens[2], out float y) &&
                float.TryParse(tokens[3], out float z))
            {
                vertices.Add(new Vector3(x, y, z));
            }
            else
            {
                Console.Error.WriteLine($"Wrong number of components at line {lineNumber}");
            }
        }

        /// <summary>
        /// Parses a texture coordinate line, e.g. "vt u v".
        /// </summary>
        private void ReadTexcoord(string line, int lineNumber)
        {
            var tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            // tokens[0] is "vt", so we need tokens[1..2] for u, v
            if (tokens.Length < 3)
            {
                Console.Error.WriteLine($"Wrong number of components at line {lineNumber}");
                return;
            }

            if (float.TryParse(tokens[1], out float u) &&
                float.TryParse(tokens[2], out float v))
            {
                texcoords.Add(new Vector2(u, v));
            }
            else
            {
                Console.Error.WriteLine($"Wrong number of components at line {lineNumber}");
            }
        }

        /// <summary>
        /// Parses a normal line, e.g. "vn nx ny nz".
        /// </summary>
        private void ReadNormal(string line, int lineNumber)
        {
            var tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            // tokens[0] is "vn", so tokens[1..3] are the normal coords
            if (tokens.Length < 4)
            {
                Console.Error.WriteLine($"Wrong number of components at line {lineNumber}");
                return;
            }

            if (float.TryParse(tokens[1], out float x) &&
                float.TryParse(tokens[2], out float y) &&
                float.TryParse(tokens[3], out float z))
            {
                normals.Add(new Vector3(x, y, z));
            }
            else
            {
                Console.Error.WriteLine($"Wrong number of components at line {lineNumber}");
            }
        }

        /// <summary>
        /// Parses a face line, e.g. "f v1/t1/n1 v2/t2/n2 v3/t3/n3 ..."
        /// </summary>
        private void ReadFace(string line, int lineNumber)
        {
            // Example: "f 1/1/1 2/2/2 3/3/3"
            var tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            // tokens[0] should be "f", so the rest are face components
            if (tokens.Length < 4)
            {
                Console.Error.WriteLine($"Face at line {lineNumber} does not have enough vertices.");
                return;
            }

            // We will read triplets of face vertices (triangulate)
            // The standard .obj face is at least 3 vertices.
            // We read first three components as one triangle.
            FaceVertex fv1 = new FaceVertex();
            FaceVertex fv2 = new FaceVertex();
            FaceVertex fv3 = new FaceVertex();

            int triangleIndex = faceVertices.Count / 3;
            fv1.ti = (uint)triangleIndex; fv1.iti = 2;
            fv2.ti = (uint)triangleIndex; fv2.iti = 1;
            fv3.ti = (uint)triangleIndex; fv3.iti = 0;

            bool fv1Ok = ReadFaceVertex(fv1, tokens[1]);
            bool fv2Ok = ReadFaceVertex(fv2, tokens[2]);
            bool fv3Ok = ReadFaceVertex(fv3, tokens[3]);

            if (fv1Ok && fv2Ok && fv3Ok)
            {
                faceVertices.Add(fv1);
                faceVertices.Add(fv2);
                faceVertices.Add(fv3);
            }

            // If there are more than 3 face components, you might want to triangulate 
            // further here. For example, a quad "f v1/t1/n1 v2/t2/n2 v3/t3/n3 v4/t4/n4" 
            // might need to form two triangles. 
            // This code only strictly handles the first 3 for a triangle.
        }

        /// <summary>
        /// Helper that reads a single face vertex part (e.g. "v/t/n").
        /// </summary>
        private bool ReadFaceVertex(FaceVertex face, string component)
        {
            // If there's no '/', it means only a vertex index
            if (!component.Contains("/"))
            {
                // Only vertex index
                if (uint.TryParse(component, out uint vertexIndex))
                {
                    face.vi = vertexIndex;
                    return true;
                }
                else
                {
                    Console.Error.WriteLine($"Invalid face format at line {lineNumber}: {component}");
                    return false;
                }
            }
            else
            {
                // The typical format is v/t/n
                // We'll use the faceRegex to parse
                Match m = faceRegex.Match(component);
                if (!m.Success)
                {
                    Console.Error.WriteLine($"Invalid face format at line {lineNumber}: {component}");
                    return false;
                }

                // Group 1: v, Group 2: t, Group 3: n
                for (int i = 1; i < m.Groups.Count; i++)
                {
                    string val = m.Groups[i].Value;
                    if (string.IsNullOrEmpty(val)) continue;

                    if (!uint.TryParse(val, out uint number))
                        continue;

                    switch (i)
                    {
                        case 1: face.vi = number; break;
                        case 2: face.tci = number; break;
                        case 3: face.ni = number; break;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Builds the final (normalized) buffers: 
        /// oIndexBuffer, oVertices, oTexcoords, oNormals
        /// based on the collected FaceVertex structures.
        /// </summary>
        public void BuildOutputMesh()
        {
            // Sort faceVertices by (vi, tci, ni)
            faceVertices.Sort();

            uint currentVertexIndex = 0;
            uint fvi = uint.MaxValue;
            uint fti = uint.MaxValue;
            uint fni = uint.MaxValue;

            // Create index buffer, filled with max to start
            oIndexBuffer = Enumerable.Repeat(uint.MaxValue, faceVertices.Count).ToList();

            // Traverse all face vertices and build the buffers
            for (int i = 0; i < faceVertices.Count; i++)
            {
                FaceVertex face = faceVertices[i];

                // If this is a new (vi, tci, ni) combination, add a new entry in output arrays
                if (face.vi != fvi || face.tci != fti || face.ni != fni)
                {
                    fvi = face.vi;
                    fti = face.tci;
                    fni = face.ni;

                    // Subtract 1 because .obj indices are 1-based
                    if (fvi > 0)
                    {
                        oVertices.Add(vertices[(int)fvi - 1]);
                    }
                    else
                    {
                        // If an index is 0 or out of range, handle gracefully 
                        oVertices.Add(Vector3.Zero);
                    }

                    if (fti > 0)
                    {
                        oTexcoords.Add(texcoords[(int)fti - 1]);
                    }
                    else
                    {
                        // Handle missing texture coordinate
                        oTexcoords.Add(Vector2.Zero);
                    }

                    if (fni > 0)
                    {
                        oNormals.Add(normals[(int)fni - 1]);
                    }
                    else
                    {
                        // Handle missing normal
                        oNormals.Add(Vector3.Zero);
                    }

                    currentVertexIndex++;
                }

                // face.ti is the triangle index, face.iti is offset in the triangle
                int fIndex = (int)(face.ti * 3 + face.iti);
                oIndexBuffer[fIndex] = currentVertexIndex - 1;
            }
        }

        #region Getters

        public IReadOnlyList<uint> GetOIndexBuffer() => oIndexBuffer;
        public uint GetNumVertices() => (uint)oVertices.Count;
        public IReadOnlyList<Vector3> GetOVertices() => oVertices;
        public IReadOnlyList<Vector2> GetOTexcoords() => oTexcoords;
        public IReadOnlyList<Vector3> GetONormals() => oNormals;

        #endregion
    }
}