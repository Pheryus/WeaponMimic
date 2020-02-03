using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour {

    public GameObject playerGhostPrefab;

    public int dashGhostClones, wallJumpGhostClones, wallJumpGhostDuration;

    List<GameObject> ghostList = new List<GameObject>();

    Player player;

    SpriteRenderer sprite;

    private void Start() {
        player = GetComponent<Player>();
        sprite = GetComponent<SpriteRenderer>();
    }


    public void CreateGhost(bool dashClones = true) {
        StartCoroutine(CoroutineCreateGhost(dashClones));
    }

    IEnumerator CoroutineCreateGhost(bool dashClones) {

        int iterations = dashClones ? dashGhostClones : wallJumpGhostClones;

        for (int i = 0; i < iterations; i++) {
            int framesBetweenGhosts;

            if (dashClones) framesBetweenGhosts = player.TotalDashDuration() / iterations;
            else framesBetweenGhosts = wallJumpGhostDuration / iterations;

            for (int j = 0; j < framesBetweenGhosts; j++) {
                yield return null;
            }
            GameObject go = Instantiate(playerGhostPrefab, transform.position, Quaternion.identity);
            SpriteRenderer goSprite = go.GetComponent<SpriteRenderer>();
            goSprite.sprite = sprite.sprite;
            goSprite.flipX = sprite.flipX;
            ghostList.Add(go);
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = ghostList.Count -1; i >= 0; i--) {
            Destroy(ghostList[i]);
        }
        ghostList.Clear();
    }

}
