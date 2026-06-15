import requests
import json
from datetime import datetime

class BrowserBehaviorTester:
    def __init__(self):
        self.test_url = "http://tes-crasflt.intra.lixil.co.jp/train/sapfront/train/scripts/PrefabTokuchujuchutoroku.asp"

    def test_empty_post(self):
        """测试空POST请求"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试空POST请求...")
        try:
            response = requests.post(self.test_url, data="", timeout=10)
            print(f"空POST请求状态码: {response.status_code}")
            print(f"响应内容: {response.text[:500]}")
            return response
        except Exception as e:
            print(f"空POST请求异常: {str(e)}")
            return None

    def test_with_user_agent(self):
        """测试使用浏览器User-Agent"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试使用浏览器User-Agent...")
        try:
            headers = {
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
            }
            response = requests.get(self.test_url, headers=headers, timeout=10)
            print(f"使用浏览器UA的GET请求状态码: {response.status_code}")
            print(f"响应内容: {response.text[:500]}")
            return response
        except Exception as e:
            print(f"使用浏览器UA请求异常: {str(e)}")
            return None

    def test_with_cookies(self):
        """测试带Cookie的请求"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试带Cookie的请求...")
        try:
            # 尝试使用之前测试中获得的Cookie
            cookies = {
                'ASPSESSIONIDQCSBDBCS': 'PKDEHDPDFCDNAAEKEOCCKDAG',
                'UqZBpD3n3iXPAw1X': 'v1CLF2gwSDMOT'
            }
            response = requests.get(self.test_url, cookies=cookies, timeout=10)
            print(f"带Cookie的GET请求状态码: {response.status_code}")
            print(f"响应内容: {response.text[:500]}")
            return response
        except Exception as e:
            print(f"带Cookie请求异常: {str(e)}")
            return None

    def test_different_content_types(self):
        """测试不同的Content-Type"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试不同的Content-Type...")
        content_types = [
            'application/json',
            'application/x-www-form-urlencoded',
            'text/plain',
            'multipart/form-data',
            'text/html'
        ]
        
        for content_type in content_types:
            print(f"\n尝试Content-Type: {content_type}")
            try:
                headers = {'Content-Type': content_type}
                response = requests.post(self.test_url, headers=headers, data="", timeout=10)
                print(f"状态码: {response.status_code}")
                if response.status_code != 500:
                    print(f"成功! Content-Type: {content_type}")
                    print(f"响应内容: {response.text[:500]}")
                    return response
            except Exception as e:
                print(f"Content-Type {content_type} 请求异常: {str(e)}")
        
        return None

    def analyze_error_pattern(self):
        """分析错误模式"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 分析错误模式...")
        print("从错误信息分析:")
        print("1. 错误发生在: /train/sapfront/include/JsonParser.asp, 行 143")
        print("2. 错误类型: Microsoft VBScript 运行时错误 '800a221c'")
        print("3. 错误描述: No JSON object could be ScanOnced")
        print("\n可能的原因:")
        print("- 接口期望接收JSON数据，但没有正确处理空请求")
        print("- JSON解析器配置有问题")
        print("- 服务器端代码逻辑错误")
        print("- 可能需要特定的请求头或认证")

    def run_all_tests(self):
        """运行所有测试"""
        print("=" * 80)
        print("浏览器行为模拟测试")
        print("=" * 80)
        print(f"目标URL: {self.test_url}")
        
        results = {}
        
        results['empty_post'] = self.test_empty_post()
        results['browser_ua'] = self.test_with_user_agent()
        results['with_cookies'] = self.test_with_cookies()
        results['content_types'] = self.test_different_content_types()
        
        self.analyze_error_pattern()
        
        return results

def main():
    tester = BrowserBehaviorTester()
    results = tester.run_all_tests()
    
    print("\n" + "=" * 80)
    print("测试总结")
    print("=" * 80)
    
    success_count = 0
    for test_name, result in results.items():
        if result and result.status_code == 200:
            status = "✅ 成功"
            success_count += 1
        else:
            status = "❌ 失败"
        print(f"{test_name}: {status}")
    
    print(f"\n成功测试数量: {success_count}/{len(results)}")
    
    if success_count == 0:
        print("\n💡 建议:")
        print("1. 接口本身可能存在问题，需要开发团队检查")
        print("2. 可能需要特定的认证或会话")
        print("3. 检查接口文档确认正确的调用方式")
        print("4. 联系系统管理员确认接口状态")

if __name__ == "__main__":
    main()
