using UnityEngine;

namespace Bullets
{
    public class NotesController : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private Color noteColor1;
        [SerializeField] private Color noteColor2;

        public void SpawnNotes(int notesToSpawn)
        {
            NotesPool pool = NotesPool.Instance;
            
            float angleBetweenNotes = Mathf.PI * 2 / notesToSpawn;
            float offsetRotation = Random.Range(0, Mathf.PI * 2);
            
            for (int i = 0; i < notesToSpawn; i++)
            {
                Bullet note = pool.GetNote();

                note.GetComponentInChildren<SpriteRenderer>().color = i % 2 == 0 ? noteColor1 : noteColor2;
                
                float phase = angleBetweenNotes * i + offsetRotation;
                
                note.transform.position = transform.position + new Vector3(Mathf.Sin(phase), 0, Mathf.Cos(phase)) * radius;
                
                note.transform.rotation = Quaternion.Euler(0, phase * Mathf.Rad2Deg, 0);
                note.transform.GetChild(0).rotation = Quaternion.identity; // make sure the child rotation isn't affected
                
                note.Launch();
            }
        }
    }
}