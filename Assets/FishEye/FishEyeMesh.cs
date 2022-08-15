using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FishEyeMesh : MonoBehaviour {

    private int vAngleStart = 0;
    private int vAngleEnd = 120;
    private int vAngleValid = 78;
    private int vAngleAdjustOffset = 70;
    
    private int hAngleStart = 0;
    private int hAngleEnd = 360;
    private int video_angleSpan = 2;
    private float video_rx = 4.5f;
    private float video_ry = 8.0f;
    private float video_rz = -23.0f;
    private float video_r_t = 0.5f;
    private float video_offsetX = 0.5f;
    private float video_offsetY = -1.5f;
    private float video_offsetZ = 1.0f;

    private double PI = 3.14159265358979323846264338327950288419716939937510582097494459230781640628;

    private float X_Size = 1920.0f;
    private float Y_Size = 1080.0f;

    private float FX = 829.409f;
    private float FY = 829.149f;

    private float PX = 1012.39f;
    private float PY = 520.333f;
    private float K1 = -0.071889f;
    private float K2 = -0.00260384f;
    private float K3 = -0.00597039f;
    private float K4 = 0.00153386f;

    private Vector3 mRotation = new Vector3(180, 0, 0);
    private Vector3 mPosition = new Vector3(0, 0, 0);
    private Vector3 mScale = new Vector3(1.92f, 1.08f, 1);

    private float XCoordinate(int hAngle, int vAngle, float rx, float offset) {
        float c = rx * (float) (Mathf.Sin(toRadians(vAngle)) * Mathf.Cos(toRadians(hAngle)));
        return c + offset;
    }

    private float YCoordinate(int hAngle, int vAngle, float ry, float offset) {
        float c = ry * (float) (Mathf.Sin(toRadians(vAngle)) * Mathf.Sin(toRadians(hAngle)));
        return c + offset;
    }

    private float ZCoordinate(int hAngle, int vAngle, float rz, float offset) {
        float c = rz * (float) (Mathf.Cos(toRadians(vAngle)));
        return c + offset;
    }

    private float FishEyeCorrection(int angle) {
        float theta = toRadians(angle);
        return theta * (1 + K1 * Mathf.Pow(theta, 2) + K2 * Mathf.Pow(theta, 4)
            + K3 * Mathf.Pow(theta, 6) + K4 * Mathf.Pow(theta, 8));
    }

    private float TXCoordinate(int hAngle, int vAngle, float r_t, bool fisheye, bool span) {
        float f;
        if (fisheye) {
        	//workarund to make side window at least has content
        	if(vAngle > vAngleValid || (!span && (vAngle == vAngleValid)))
        		vAngle = vAngleAdjustOffset + (vAngle - vAngleAdjustOffset) * (vAngleValid - vAngleAdjustOffset)/(vAngleEnd - vAngleAdjustOffset);
        
            f = (float) (0.5 + ((PX - (X_Size/2))/X_Size) + r_t * FX
                * Mathf.Tan(FishEyeCorrection(vAngle)) * Mathf.Cos(toRadians(hAngle))/X_Size);
        } else {
            f = (float)( r_t * Mathf.Sin(toRadians(vAngle)) * Mathf.Cos(toRadians(hAngle)));
            if(vAngle <= 90)
                f += 0.5f ;
            else if(hAngle > 90 && hAngle < 270)
                f = 0.0f - f;
            else
                f = 1.0f - f;
        }
        return f;
    }

    private float TYCoordinate(int hAngle, int vAngle, float r_t, bool fisheye, bool span) {
        float f;
        if (fisheye) {
        	//workarund to make side window at least has content
        	if(vAngle > vAngleValid || (!span && (vAngle == vAngleValid)))
        		vAngle = vAngleAdjustOffset + (vAngle - vAngleAdjustOffset) * (vAngleValid - vAngleAdjustOffset)/(vAngleEnd - vAngleAdjustOffset);

            f = (float) (0.5 + ((PY - (Y_Size/2))/Y_Size) - r_t * FY
                * Mathf.Tan(FishEyeCorrection(vAngle)) * Mathf.Sin(toRadians(hAngle))/Y_Size);
        } else {
            f = (float) (0.5 - r_t * Mathf.Sin(toRadians(vAngle)) * Mathf.Sin(toRadians(hAngle)));
        }
        return f;
    }

    private float toRadians(double vAngle) {
        double e;
        double sign = ( vAngle < 0.0 ) ? -1.0 : 1.0;
        if(vAngle < 0)
            vAngle = vAngle * ( -1.0 );
        e = sign * vAngle * PI / 180;
        return (float) e;
    }

    private Mesh initMesh(int angleSpan, float rx, float ry, float rz, float r_tx, float r_ty,
            float offsetX, float offsetY, float offsetZ, bool fisheye) {
        int block_count = ((vAngleEnd - vAngleStart) / angleSpan) * ((hAngleEnd - hAngleStart)/angleSpan);
        Vector3[] vertices = new Vector3[block_count * 4];
        Vector2[] uv = new Vector2[block_count * 4];
        int[] tris = new int[block_count * 6];
        int block_index = 0;
        for (int hAngle = hAngleStart; hAngle < hAngleEnd; hAngle = hAngle + angleSpan) {
            for (int vAngle = vAngleStart; vAngle < vAngleEnd; vAngle = vAngle + angleSpan) {
                float x0 = XCoordinate(hAngle, vAngle, rx, offsetX);
                float y0 = YCoordinate(hAngle, vAngle, ry, offsetY);
                float z0 = ZCoordinate(hAngle, vAngle, rz, offsetZ);

                float tX0 = TXCoordinate(hAngle, vAngle, r_tx, fisheye, false);
                float tY0 = TYCoordinate(hAngle, vAngle, r_ty, fisheye, false);

                float x1 = XCoordinate(hAngle + angleSpan, vAngle, rx, offsetX);
                float y1 = YCoordinate(hAngle + angleSpan, vAngle, ry, offsetY);
                float z1 = ZCoordinate(hAngle + angleSpan, vAngle, rz, offsetZ);

                float tX1 = TXCoordinate(hAngle + angleSpan, vAngle, r_tx, fisheye, false);
                float tY1 = TYCoordinate(hAngle + angleSpan, vAngle, r_ty, fisheye, false);

                float x2 = XCoordinate(hAngle + angleSpan, vAngle + angleSpan, rx, offsetX);
                float y2 = YCoordinate(hAngle + angleSpan, vAngle + angleSpan, ry, offsetY);
                float z2 = ZCoordinate(hAngle + angleSpan, vAngle + angleSpan, rz, offsetZ);

                float tX2 = TXCoordinate(hAngle + angleSpan, vAngle + angleSpan, r_tx, fisheye, true);
                float tY2 = TYCoordinate(hAngle + angleSpan, vAngle + angleSpan, r_ty, fisheye, true);

                float x3 = XCoordinate(hAngle, vAngle + angleSpan, rx, offsetX);
                float y3 = YCoordinate(hAngle, vAngle + angleSpan, ry, offsetY);
                float z3 = ZCoordinate(hAngle, vAngle + angleSpan, rz, offsetZ);

                float tX3 = TXCoordinate(hAngle, vAngle + angleSpan, r_tx, fisheye, true);
                float tY3 = TYCoordinate(hAngle, vAngle + angleSpan, r_ty, fisheye, true);

                int index_vertices = block_index * 4;
                vertices[index_vertices++] = new Vector3(x0, y0, z0);
                vertices[index_vertices++] = new Vector3(x1, y1, z1);
                vertices[index_vertices++] = new Vector3(x2, y2, z2);
                vertices[index_vertices++] = new Vector3(x3, y3, z3);
                int index_textureCoods = block_index * 4;
                uv[index_textureCoods++] = new Vector2(tX0, tY0);
                uv[index_textureCoods++] = new Vector2(tX1, tY1);
                uv[index_textureCoods++] = new Vector2(tX2, tY2);
                uv[index_textureCoods++] = new Vector2(tX3, tY3);
                if(tX0 >= 0.0f && tX1 >= 0.0f && tX2 >= 0.0f && tX3 >= 0.0f && tX0 <= 1.0f
                        && tX1 <= 1.0f && tX2 <= 1.0f && tX3 <= 1.0f && tY0 >= 0.0f && tY1 >= 0.0f
                        && tY2 >= 0.0f && tY3 >= 0.0f && tY0 <= 1.0f && tY1 <= 1.0f && tY2 <= 1.0f
                        && tY3 <= 1.0f) {
                    int index_drawOrder = block_index * 6;
                    tris[index_drawOrder++] = block_index * 4 + 1;
                    tris[index_drawOrder++] = block_index * 4 + 0;
                    tris[index_drawOrder++] = block_index * 4 + 3;
                    tris[index_drawOrder++] = block_index * 4 + 1;
                    tris[index_drawOrder++] = block_index * 4 + 3;
                    tris[index_drawOrder++] = block_index * 4 + 2;
                }
                block_index++;
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        return mesh;
    }

    void Start() {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = initMesh(video_angleSpan, video_rx, video_ry, video_rz, video_r_t, video_r_t,
            video_offsetX, video_offsetY, video_offsetZ, true);
        transform.localEulerAngles = mRotation;
        transform.localPosition = mPosition;
        transform.localScale = mScale;
    }

}