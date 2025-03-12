using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Unity에서 Event Manager는 Observer Pattern이다.
/// 사용하려면 Interface IListener를 추가해야 한다.
/// 또한, 구독은 정말 필요한 경우에 사용해야 한다.
/// 그렇지 않으면 구독한 모든 Object들이 적용되서 필요하지 않은 경우에도 같이 Update 된다.
/// 0624 이용성
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// Event Manager 함수 요약
    // 구독 : AddEvent
    // 구독 취소 : RemoveEvent
    // 전체 구독 취소 : RemoveEvent
    // Type별로 바뀐 내용을 알릴 함수 : NotifyEvent
    // 중복 제거할 함수 : RemoveRedundancies
    // 씬 변경시 할 함수 : ChangeScene

    // Event Type은 확실해, 그 다음에 변경할 Object나 값?
    public delegate void OnEvent(EventType eventType, object param = null);
    private Dictionary<EventType, List<OnEvent>> listeners = new Dictionary<EventType, List<OnEvent>>();

    /// <summary>
    /// OnEvent를 구독하는 함수
    /// </summary>
    /// <param name="eventType">이벤트 타입</param>
    /// <param name="Listener">OnEvent 함수를 등록</param>
    public void AddEvent(EventType eventType, OnEvent listener)
    {
        // listen List 
        List<OnEvent> listenList = null;

        // 이거 들어가나?
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
    /// 중간에 구독되어 있는 사람들에게 모두 알리는 함수
    /// </summary>
    /// <param name="eventType">이벤트 타입</param>
    /// <param name="param">바뀔 무언가</param>
    public void NotifyEvent(EventType eventType, object param = null)
    {
        List<OnEvent> listenList = null;

        // 이미 없다면 Return 해 버린다.
        if (!listeners.TryGetValue(eventType, out listenList))
            return;

        // OnEvent를 순회한다.
        for (int i = 0; i < listenList.Count; i++)
        {
            listenList?[i](eventType, param);
        }
    }

    /// <summary>
    /// Type에 구독되어 있는 것들을 전부 구독 취소하는 함수
    /// </summary>
    /// <param name="eventType">이벤트 타입</param>
    public void RemoveEvent(EventType eventType) => listeners.Remove(eventType);

    /// <summary>
    /// Type과 구독되어 있는 것을 토대로 구독 취소하는 함수
    /// </summary>
    /// <param name="eventType">이벤트 타입</param>
    /// <param name="listener">해당 Event</param>
    public void RemoveEvent(EventType eventType, OnEvent listener)
    {
        // 해당 이벤트만 구독 취소 할건데
        // Dictionary listeners 안에 있는 list<OnEvent> 를 listener랑 비교해서
        // 같다면 그거 Remove()을 하면 될 거 같다.

        if(!listeners.ContainsKey(eventType))
        {
            return;
        }

        // List 안에 포함되어 있다면 True 아니면 false
        if (listeners[eventType].Contains(listener))
        {
            listeners[eventType].Remove(listener);
        }
    }

    /// <summary>
    /// 씬이 바뀔 때 호출해야 하는 함수
    /// 설정 안 해주면 다른 씬 넘어갈때마다 생성될 것이기 때문이다.
    /// </summary>
    public void ChangeScene()
    {
        RemoveRedundancies();
    }

    /// <summary>
    /// 씬이 바뀔 때 재시작하거나 종료할 때 이 함수를 사용해서 EventType들을 전부 없애야 한다.
    /// </summary>
    public void RemoveAllEvent()
    {
        for(int i = 0; i< (int)EventType.END; i++)
        {
            listeners.Remove((EventType)i);
        }
    }

    /// <summary>
    /// 중복 제거하는 함수
    /// 씬이 바뀌면서 기존에 있던 것들이 null이 되기 때문에 삭제한다.
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
