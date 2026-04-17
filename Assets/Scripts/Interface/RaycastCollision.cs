//struct RaycastCollision
//{
//    bool found;
//    uint8_t type;
//    glm::vec3 collisionPoint;
//    glm::vec3 blockPosition;
//    glm::vec3 blockToCollisionPointDirection;
//    glm::vec3 collisionFaceNormal;
//};
using UnityEngine;
struct RaycastCollision
{
    public bool found;
    public BlockType type;
    public Vector3 collisionPoint;
    public Vector3 blockPosition;
    public Vector3 blockToCollisionPointDirection;
    public Vector3 collisionFaceNormal;
}