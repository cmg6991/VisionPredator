/// <summary>
/// Item �ʱ� ������ Item�� �Ծ��� �� Player���� interface
/// canvas�� ��ȸ�ϸ鼭 image, tmp_text�� �����ͼ� �����ۿ� �°� �����Ѵ�.
/// �ٵ� �̰� �Լ��� �ϳ� ����� �ؿ� item �����ο� �� canvas�� player�� �ִ°� ���� ������?
/// ���⿡ ���� ������ ���µ�?
/// �̸��� ��� ����?
/// 
/// Item text ������ �ʱ�ȭ, Item�̶� Player�� ��������
/// 
/// Item <=> UI Item�� ����� �� ���� �ѱ��? 
/// ���� �̰� ��� �ϳ�? �� Action, Func�� �̿��ؼ� Lamda�� �� ����ϸ� ���� �ʳ�?
/// �̰� �� ��ȣ�ۿ��ؼ� ����ϴ� �� �ƴϴϱ� interface �ʿ� ���� ��?
/// </summary>
public interface IItemUI
{
    // �ʱ⿡ ���� canvas���� �ڽĵ��� ��ȸ�ϸ鼭 image, text�� �޾ƿͼ� 
    // �����ۿ� �°� ������ ����� ���� ������?
    // �ٵ� �̰Źۿ� ������ delegate�� �´ٸ� interface�� ����ϰ� ������ 

    // Initalize������
    // canvas�� �ڽĵ��� ��ȸ�ϸ鼭 image�� �޾ƿ���
    // image���� text_tmp�� �޾ƿͼ� text�� �����Ѵ�.
    public virtual void Initalize() { }
    
    // item�� �Ծ��� �� Player 
    public virtual void PickUp() { }
}
