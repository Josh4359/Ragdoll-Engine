#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using static SplineImporter;
using static SplineData;
using System.Linq;
using System.IO;
using System.Text;
using Unity.Mathematics;

[CustomEditor(typeof(SplineImporter))]
public class SplineImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Import Spline"))
        {
            SplineImporter splineImporter = target as SplineImporter;

            splineImporter.name = splineImporter.splineData.name;

            SplineData splineData = JsonUtility.FromJson<SplineData>(splineImporter.splineData.text);

            SplineContainer splineContainer = splineImporter.GetComponent<SplineContainer>();

            foreach (Spline thisSpline in splineContainer.Splines)
                splineContainer.RemoveSpline(thisSpline);

            splineContainer.AddSpline();

            foreach (ControlPoint thisControlPoint in splineData.controlPoints)
            {
                Vector3 position = PositionToVector(thisControlPoint.position);

                Vector3 handleL = PositionToVector(thisControlPoint.handleL);

                Vector3 handleR = PositionToVector(thisControlPoint.handleR);

                Quaternion rotation = Quaternion.LookRotation(handleR - position, Vector3.up) * Quaternion.AngleAxis(-thisControlPoint.tilt, Vector3.forward);

                float3x3 rotationMatrix = new float3x3(rotation);

                splineContainer.Spline.Add(new BezierKnot()
                {
                    Position = position * splineImporter.scale,
                    Rotation = rotation,
                    TangentIn = ((Vector3)math.mul(handleL - position, rotationMatrix)) * splineImporter.scale,
                    TangentOut = ((Vector3)math.mul(handleR - position, rotationMatrix)) * splineImporter.scale
                },
                TangentMode.Broken);
            }
        }

        if (GUILayout.Button("Export Spline"))
        {
            SplineImporter splineImporter = target as SplineImporter;

            if (!splineImporter.splineData)
            {
                string path = "Assets" + EditorUtility.SaveFilePanel("Save .JSON", "", "New Spline.json", "json").Substring(Application.dataPath.Length);

                if (path.Length > 0)
                {
                    File.WriteAllBytes(path, Encoding.ASCII.GetBytes(""));

                    AssetDatabase.Refresh();

                    TextAsset textAsset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;

                    splineImporter.splineData = textAsset;
                }
                else return;
            }

            SplineContainer splineContainer = splineImporter.GetComponent<SplineContainer>();

            SplineData splineData = new SplineData();

            splineData.controlPoints = new ControlPoint[0];

            foreach (BezierKnot thisBezierKnot in splineContainer.Spline.Knots)
            {
                Position position = VectorToPosition(thisBezierKnot.Position);

                float3x3 rotationMatrix =  new float3x3(Quaternion.Inverse(thisBezierKnot.Rotation));

                Position handleL = VectorToPosition(math.mul(thisBezierKnot.TangentIn, rotationMatrix) + thisBezierKnot.Position);

                Position handleR = VectorToPosition(math.mul(thisBezierKnot.TangentOut, rotationMatrix) + thisBezierKnot.Position);

                splineData.controlPoints = splineData.controlPoints.Append(new ControlPoint()
                {
                    position = position,
                    handleL = handleL,
                    handleR = handleR
                }).ToArray();
            }

            File.WriteAllText(AssetDatabase.GetAssetPath(splineImporter.splineData), JsonUtility.ToJson(splineData, true));

            AssetDatabase.Refresh();
        }
    }
}

[RequireComponent(typeof(SplineContainer))]
public class SplineImporter : MonoBehaviour
{
    public TextAsset splineData;

    public float scale = 1;

    public static Vector3 PositionToVector(Position position)
    {
        return new Vector3(position.x, position.z, position.y);
    }

    public static Position VectorToPosition(Vector3 vector)
    {
        return new Position()
        {
            x = vector.x,
            y = vector.z,
            z = vector.y
        };
    }
}

[Serializable]
public class SplineData
{
    [Serializable]
    public struct Position
    {
        public float x;

        public float y;

        public float z;
    }

    [Serializable]
    public struct ControlPoint
    {
        public Position position;

        public Position handleL;

        public Position handleR;

        public float tilt;
    }

    public ControlPoint[] controlPoints;
}

#endif
