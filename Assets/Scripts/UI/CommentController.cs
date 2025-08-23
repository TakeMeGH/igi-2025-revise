using TMPro;
using UnityEngine;

namespace Perspective.UI
{
    public class CommentController : MonoBehaviour
    {
        [SerializeField] private TMP_Text commentText;
        
        public void SetCommentText(string comment)
        {
            commentText.text = comment;
        }
    }
}