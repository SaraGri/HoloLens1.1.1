using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedObject
{
    // Feature/Edge points of the sliced object
    private List<Vector3> m_ankerPoints;

    // Type of the sliced geometry:
    // - Triangle, Rectangle, Circle, Polygon
    public enum GeometryType { Triangle, Rectangle, Circle, Polygon };
    private GeometryType m_geometryType;

    // Ctor
    public SlicedObject(List<Vector3> ankerPoints, GeometryType geometryType)
    {
        m_ankerPoints = ankerPoints;
        m_geometryType = geometryType;
    }

    public List<Vector3> GetAnkerPoints()
    {
        return m_ankerPoints;
    }

    public void SetAnkerPoints(List<Vector3> ankerPoints)
    {
        m_ankerPoints = ankerPoints;
    }

    public GeometryType GetGeometryType()
    {
        return m_geometryType;
    }

    public void SetGeometryType(GeometryType geometryType)
    {
        m_geometryType = geometryType;
    }


}

