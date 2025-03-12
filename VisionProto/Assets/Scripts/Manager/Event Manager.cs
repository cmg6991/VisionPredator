using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Unity���� Event Manager�� Observer Pattern�̴�.
/// ����Ϸ��� Interface IListener�� �߰��ؾ� �Ѵ�.
/// ����, ������ ���� �ʿ��� ��쿡 ����ؾ� �Ѵ�.
/// �׷��� ������ ������ ��� Object���� ����Ǽ� �ʿ����� ���� ��쿡�� ���� Update �ȴ�.
/// 0624 �̿뼺
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// Event Manager �Լ� ���
    // ���� : AddEvent
    // ���� ��� : RemoveEvent
    // ��ü ���� ��� : RemoveEvent
    // Type���� �ٲ� ������ �˸� �Լ� : NotifyEvent
    // �ߺ� ������ �Լ� : RemoveRedundancies
    // �� ����� �� �Լ� : ChangeScene

    // Event Type�� Ȯ����, �� ������ ������ Object�� ��?
    public delegate void OnEvent(EventType eventType, object param = null);
    private Dictionary<EventType, List<OnEvent>> listeners = new Dictionary<EventType, List<OnEvent>>();

    /// <summary>
    /// OnEvent�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="eventType">�̺�Ʈ Ÿ��</param>
    /// <param name="Listener">OnEvent �Լ��� ���</param>
    public void AddEvent(EventType eventType, OnEvent listener)
    {
        // listen List 
        List<OnEvent> listenList = null;

        // �̰� ����?
        if (listeners.TryGetValue(eventType, out listenList))
        {
            listenList.Add(listener);
            return;
        }

        listenList = new List<OnEvent>();
        listenList.Add(listener);
        listeners.Add(eventType, listenList);
    }

    /// <summary>
    /// �߰��� �����Ǿ� �ִ� ����鿡�� ��� �˸��� �Լ�
    /// </summary>
    /// <param name="eventType">�̺�Ʈ Ÿ��</param>
    /// <param name="param">�ٲ� ����</param>
    public void NotifyEvent(EventType eventType, object param = null)
    {
        List<OnEvent> listenList = null;

        // �̹� ���ٸ� Return �� ������.
        if (!listeners.TryGetValue(eventType, out listenList))
            return;

        // OnEvent�� ��ȸ�Ѵ�.
        for (int i = 0; i < listenList.Count; i++)
        {
            listenList?[i](eventType, param);
        }
    }

    /// <summary>
    /// Type�� �����Ǿ� �ִ� �͵��� ���� ���� ����ϴ� �Լ�
    /// </summary>
    /// <param name="eventType">�̺�Ʈ Ÿ��</param>
    public void RemoveEvent(EventType eventType) => listeners.Remove(eventType);

    /// <summary>
    /// Type�� �����Ǿ� �ִ� ���� ���� ���� ����ϴ� �Լ�
    /// </summary>
    /// <param name="eventType">�̺�Ʈ Ÿ��</param>
    /// <param name="listener">�ش� Event</param>
    public void RemoveEvent(EventType eventType, OnEvent listener)
    {
        // �ش� �̺�Ʈ�� ���� ��� �Ұǵ�
        // Dictionary listeners �ȿ� �ִ� list<OnEvent> �� listener�� ���ؼ�
        // ���ٸ� �װ� Remove()�� �ϸ� �� �� ����.

        if(!listeners.ContainsKey(eventType))
        {
            return;
        }

        // List �ȿ� ���ԵǾ� �ִٸ� True �ƴϸ� false
        if (listeners[eventType].Contains(listener))
        {
            listeners[eventType].Remove(listener);
        }
    }

    /// <summary>
    /// ���� �ٲ� �� ȣ���ؾ� �ϴ� �Լ�
    /// ���� �� ���ָ� �ٸ� �� �Ѿ������ ������ ���̱� �����̴�.
    /// </summary>
    public void ChangeScene()
    {
        RemoveRedundancies();
    }

    /// <summary>
    /// ���� �ٲ� �� ������ϰų� ������ �� �� �Լ��� ����ؼ� EventType���� ���� ���־� �Ѵ�.
    /// </summary>
    public void RemoveAllEvent()
    {
        for(int i = 0; i< (int)EventType.END; i++)
        {
            listeners.Remove((EventType)i);
        }
    }

    /// <summary>
    /// �ߺ� �����ϴ� �Լ�
    /// ���� �ٲ�鼭 ������ �ִ� �͵��� null�� �Ǳ� ������ �����Ѵ�.
    /// </summary>
    private void RemoveRedundancies()
    {
        Dictionary<EventType, List<OnEvent>> newListener = new Dictionary<EventType, List<OnEvent>>();

        foreach (var item in listeners)
        {
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Value[i].Target.Equals(null))
                    item.Value.RemoveAt(i);
            }

            if (item.Value.Count > 0)
                newListener.Add(item.Key, item.Value);
        }

        listeners = newListener;
    }
}
