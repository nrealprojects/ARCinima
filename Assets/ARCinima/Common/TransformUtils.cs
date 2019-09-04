using UnityEngine;

public static class TransformUtils
{
    public static Matrix4x4 GetTMatrix(Vector3 position, Quaternion rotation)
    {
        return Matrix4x4.TRS(position, rotation, Vector3.one);
    }

    public static Matrix4x4 GetTMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        return Matrix4x4.TRS(position, rotation, scale);
    }

    public static Vector3 GetPositionFromTMatrix(Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;

        return position;
    }

    public static Quaternion GetRotationFromTMatrix(Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static void LogPosition(string message, Vector3 position)
    {
        Debug.LogError(message);
        Debug.LogError("x: " + position.x + "; y: " + position.y + "; z: " + position.z);
    }

    public static void LogRotation(string message, Quaternion rotation)
    {
        Debug.LogError(message);
        Debug.LogError("x: " + rotation.x + "; y: " + rotation.y + "; z: " + rotation.z + "; w: " + rotation.w);
    }

    public static void LogTransformMatrix(string message, Matrix4x4 T)
    {
        Debug.LogError(message);
        Debug.LogError(T);
    }

	public static Vector3 ConvertPosition(Vector3 vec)
	{
		// Convert to left-handed
		return new Vector3((float)vec.x, (float)vec.y, (float)-vec.z);
	}
	public static Quaternion ConvertOrientation(Quaternion quat)
	{
		// Convert to left-handed
		return new Quaternion(-(float)quat.x, -(float)quat.y, (float)quat.z, (float)quat.w);
	}
}
