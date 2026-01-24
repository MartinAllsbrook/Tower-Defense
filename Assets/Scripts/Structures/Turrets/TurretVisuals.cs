using UnityEngine;

class TurretVisuals : MonoBehaviour
{
    [SerializeField] SpriteRenderer foundationSprite;
    [SerializeField] SpriteRenderer turretSprite;
    [SerializeField] SpriteRenderer cannonSprite;

    public void SetColor(Color color)
    {
        foundationSprite.color = color;
        turretSprite.color = color;
        cannonSprite.color = color;
    }   
}