using UnityEngine;

public class PoolObject : MonoBehaviour
{
    // PoolObject를 상속받는 자식 클래스로 다운 캐스팅
    public T ReturnMyComponent<T>() where T : PoolObject
    {
        return this as T;
    }

}