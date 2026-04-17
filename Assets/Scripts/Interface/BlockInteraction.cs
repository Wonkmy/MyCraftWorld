using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BlockInteraction : MonoBehaviour
{
    float raycast_distance = 12.5f;
    float raycast_precision = 0.01f;
    bool instant_breaking = false;

    private void Update()
    {
        #region  SetBlock 函数 GetBlockID 函数 没有任何问题
        // WorldGen.Instance.SetBlock 函数没有任何问题
        // WorldGen.Instance.GetBlockID 函数没有任何问题
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    BlockType blockType = WorldGen.Instance.GetBlockID(12.5f, 10.5f, 2.5f);

        //    if(blockType!= BlockType.Air)
        //    {
        //        WorldGen.Instance.SetBlock(12.5f, 10.5f, 2.5f, -1);
        //    }
        //    else
        //    {
        //        Debug.LogError($"Block Type是{blockType}");
        //    }
        //}
        #endregion

        if (Input.GetMouseButtonDown(0))
        {
            RaycastCollision collision;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Raycast(ray.origin, ray.direction, raycast_distance, out collision);
            if (collision.found)
            {
                Debug.Log("Block ID: " + collision.type);
                Debug.Log("Collision Point: " + collision.collisionPoint);
                Debug.Log("Block Position: " + collision.blockPosition);
                //Debug.Log("Collision Face Normal: " + collision.collisionFaceNormal);
                WorldGen.Instance.SetBlock((int)collision.blockPosition.x, (int)collision.blockPosition.y, (int)collision.blockPosition.z, -1);
            }
        }
    }
    void Raycast(Vector3 p_source,Vector3 p_direction,float _maxDistance,out RaycastCollision p_out)
    {
        RaycastCollision result = new RaycastCollision();
        result.found = false;
        result.type = 0;
        result.collisionPoint = Vector3.zero;
        result.blockPosition = Vector3.zero;
        result.collisionFaceNormal = Vector3.zero;
        p_direction = p_direction.normalized;
        for (int distance = 0; distance < (_maxDistance / raycast_precision) && !result.found; ++distance)
        {
            Vector3 destination = p_source + p_direction * (distance * raycast_precision);
            //Debug.Log($"destination:{destination}=p_source{p_source}+p_direction{p_direction}*distance{distance} * raycast_precision{raycast_precision}");
            //Debug.DrawRay(p_source, p_direction * raycast_distance, Color.red, 5f);
            BlockType blockType = WorldGen.Instance.GetBlockID(destination.x, destination.y, destination.z);
            if (blockType == BlockType.Air)
                continue;

            result.type = blockType;

            result.found = true;
            result.collisionPoint = destination;
            result.blockPosition.x = Mathf.Round(destination.x);
            result.blockPosition.y = Mathf.Round(destination.y);
            result.blockPosition.z = Mathf.Round(destination.z);
            result.blockToCollisionPointDirection = Vector3.Normalize(result.collisionPoint - result.blockPosition);

            Vector3 unsignedDirectionFromBlockToImpact = new Vector3(Mathf.Abs(result.blockToCollisionPointDirection.x), Mathf.Abs(result.blockToCollisionPointDirection.y), Mathf.Abs(result.blockToCollisionPointDirection.z));
            if (unsignedDirectionFromBlockToImpact.x > unsignedDirectionFromBlockToImpact.y && unsignedDirectionFromBlockToImpact.x > unsignedDirectionFromBlockToImpact.z)
                result.collisionFaceNormal = result.blockToCollisionPointDirection.x > 0 ? new Vector3(1f, 0f, 0f) : new Vector3(-1f, 0f, 0f);
            else if (unsignedDirectionFromBlockToImpact.y > unsignedDirectionFromBlockToImpact.x && unsignedDirectionFromBlockToImpact.y > unsignedDirectionFromBlockToImpact.z)
                result.collisionFaceNormal = result.blockToCollisionPointDirection.y > 0 ? new Vector3(0f, 1f, 0f) : new Vector3(0f, -1f, 0f);
            else
                result.collisionFaceNormal = result.blockToCollisionPointDirection.z > 0 ? new Vector3(0f, 0f, 1f) : new Vector3(0f, 0f, -1f);
        }
        p_out = result;
    }
}
