using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Bullets
{
    public class NotesPool : SceneSingleton<NotesPool>
    {
        [SerializeField] private Bullet notePrefab;
        private ObjectPool<Bullet> _notesPool;

        private void Start()
        {
            _notesPool = new ObjectPool<Bullet>(
                () =>
                {
                    Bullet note = Instantiate(notePrefab, transform);
                    note.SetPool(_notesPool);
                    return note;
                },
                note =>
                {
                    note.gameObject.SetActive(true);
                    note.transform.SetParent(transform);
                },
                note => note.gameObject.SetActive(false),
                note => Destroy(note.gameObject),
                false,
                30
            );
        }

        public Bullet GetNote() => _notesPool.Get();
    }
}