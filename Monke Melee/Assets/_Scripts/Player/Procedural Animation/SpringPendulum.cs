using UnityEngine;

public class SpringPendulum : MonoBehaviour
{
    public GameObject Pivot;
    public GameObject Mass1;
    public GameObject Mass2;

    void Update()
    {
        Vector3 forwardM1 = Vector3.Cross(Mass1.transform.right, (Pivot.transform.position - Mass1.transform.position).normalized);
        Vector3 forwardM2 = Vector3.Cross(Mass2.transform.right, (Mass1.transform.position - Mass2.transform.position).normalized);

        Mass1.transform.rotation = Quaternion.LookRotation(forwardM1, (Pivot.transform.position - Mass1.transform.position).normalized);
        Mass2.transform.rotation = Quaternion.LookRotation(forwardM2, (Mass1.transform.position - Mass2.transform.position).normalized);

        Debug.DrawRay(Mass1.transform.position, Mass1.transform.up, Color.green);
        Debug.DrawRay(Mass1.transform.position, Mass1.transform.forward, Color.blue);

        Debug.DrawRay(Mass2.transform.position, Mass2.transform.up, Color.green);
        Debug.DrawRay(Mass2.transform.position, Mass2.transform.forward, Color.blue);
    }

    [ContextMenu("Reset Pendulum")]
    public void ResetPendulum()
    {
        Mass1.transform.position = Vector3.zero;
        Mass1.transform.rotation = Quaternion.identity;
        Mass1.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        Mass2.transform.position = Vector3.zero;
        Mass2.transform.rotation = Quaternion.identity;
        Mass2.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}