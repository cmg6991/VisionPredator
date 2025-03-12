/// <summary>
/// Item 초기 정보와 Item을 먹었을 때 Player에게 interface
/// canvas를 순회하면서 image, tmp_text를 가져와서 아이템에 맞게 설정한다.
/// 근데 이거 함수만 하나 만들고 밑에 item 구현부에 저 canvas랑 player를 넣는게 맞지 않을까?
/// 여기에 있을 이유가 없는듯?
/// 이름을 어떻게 짓냐?
/// 
/// Item text 정보를 초기화, Item이랑 Player랑 연관관계
/// 
/// Item <=> UI Item을 닿았을 때 값을 넘긴다? 
/// 굳이 이거 써야 하나? 걍 Action, Func을 이용해서 Lamda를 잘 사용하면 되지 않나?
/// 이게 막 상호작용해서 사용하는 건 아니니까 interface 필요 없을 듯?
/// </summary>
public interface IItemUI
{
    // 초기에 위에 canvas에서 자식들을 순회하면서 image, text를 받아와서 
    // 아이템에 맞게 세팅을 해줘야 하지 않을까?
    // 근데 이거밖에 없으면 delegate가 맞다며 interface를 사용하고 싶으면 

    // Initalize에서는
    // canvas의 자식들을 순회하면서 image를 받아오고
    // image에서 text_tmp를 받아와서 text를 설정한다.
    public virtual void Initalize() { }
    
    // item을 먹었을 때 Player 
    public virtual void PickUp() { }
}
