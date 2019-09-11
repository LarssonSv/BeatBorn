using UnityEngine;

public interface IDamageable {
    void TakeDamage(int damage = 1, RhythmGrade grade = RhythmGrade.Miss,Transform attacker = null, float missMultiplier = 1);
    void Die();
}